using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using AgaQ.Bricks.Joints;

namespace AgaQ.Bricks.Positioners
{
	/// <summary>
	/// Helper class to establish brick postion during dragging.
	/// It's based on simple positioner and try to join bricks.
	/// </summary>
	public class JoinablePositioner : SimplePositioner
    {
        public const float positionAccuracy = 0.005f;
        public const float rotationAccuracy = 0.5f;
        const float jointColisionDistance = 0.15f; //joints can't be positioned in distance betweeen positionAccuracy and this.
        const float jointsDistance = 0.25f; //distance between two standard joints in brick (scale 1)

        public override ProposedPosition GetPosition(Vector3 mousePosition, bool snapToGrid, float gridSize)
		{
            var simplePosition = base.GetPosition(mousePosition, false, gridSize);
            brick.transform.position = simplePosition.position;

            if (!(brick is AgaQBrick))
                return simplePosition;

            //calculate distance betweeen two standard joints in screen space.
            var joint1Pos = simplePosition.position + cam.transform.right * jointsDistance * brick.transform.localScale.x;
            var joint2Pos = simplePosition.position - cam.transform.right * jointsDistance * brick.transform.localScale.x;
            var jointsScreenDistance = (cam.WorldToScreenPoint(joint1Pos) - cam.WorldToScreenPoint(joint2Pos)).magnitude * 0.5f;

            //transform our moving brick joints postions to screen space
			var brickJoints = ProjectionTool.ProjectJoints(brick as AgaQBrick);

			//get possible joints
            var gameObjectsBehind = GetOtherBricks(mousePosition);
            var possibleJoints = GetClosetsJoins (brickJoints, gameObjectsBehind, jointsScreenDistance);

            //iterate over possible joints
            foreach (var possibleJoint in possibleJoints)
			{
                if (CheckJoint(possibleJoint, snapToGrid))
                {
                    return new ProposedPosition(brick.transform, true, true);
                }
			}

            if (!snapToGrid)
                return simplePosition;
            
            return base.GetPosition(mousePosition, snapToGrid, gridSize);
		}

		/// <summary>
		/// Get joints from other brics that are close enouth to try join brick with.
		/// List is sorted by distance after projeting to the plane.
		/// </summary>
        /// <param name="projectedDragingBrickJoints"></param>
        /// <param name="positionTolerance">Position tolerance.</param>
		/// <returns>The closets joins.</returns>
        PossibleJoint[] GetClosetsJoins(ProjectedJoint[] projectedDragingBrickJoints, GameObject[] gameObjects, float positionTolerance)
		{
            List<PossibleJoint> possibleJoints = new List<PossibleJoint> ();

			//iterate over all colliders
            foreach (var otherGameObject in gameObjects)
			{
                //skip grid and itself
                if (otherGameObject.tag == "background" || otherGameObject == this.brick.gameObject)
                    continue;
                
				//check if collider is AgaQ brick
                var otherBrick = otherGameObject.GetComponent<AgaQBrick> ();
                if (otherBrick == null)
                {
                    //try to get AgaQScript from parent
                    otherBrick = otherGameObject.GetComponentInParent<AgaQBrick>();
                    if (otherBrick == null)
                        continue;
                }

				//iterate over all brick joints
                foreach (var otherBrickJoint in otherBrick.joints)
				{
                    if (otherBrickJoint.other == null)
                    {
                        foreach (var projectedDragingBrickJoint in projectedDragingBrickJoints)
                        {
                            var projectedJoint = ProjectionTool.ProjectJoint(otherBrickJoint);
                            if (projectedDragingBrickJoint.Compare(projectedJoint, positionTolerance))
                                possibleJoints.Add(new PossibleJoint(projectedDragingBrickJoint, projectedJoint));
                        }
                    }
				}
			}

            //draw possible joints in editor for debuging purpose
            #if UNITY_EDITOR
            foreach (var j in possibleJoints)
            {
                DebugExtension.DebugCircle(
                    j.projectedOtherJoint.joint.transform.position,
                    Color.red,
                    0.1f,
                    02.2f,
                    false
                );
                //Debug.DrawLine(
                //    j.projectedBrickJoint.joint.transform.position,
                //    j.projectedOtherJoint.joint.transform.position,
                //    Color.red,
                //    0.2f,
                //    false
                //);
            }
            #endif

            //possibleJoints.OrderBy (x => x.cameraVectorDistance); // order joints by distance to camera
            //possibleJoints.OrderBy (x => x.projectedDistance); // order joints by distance after project

            //sort by brick to camera distance than by projected distance
            possibleJoints.Sort((x, y) => {
                if (x.projectedOtherJoint.joint.parentBrick != y.projectedOtherJoint.joint.parentBrick)
                    return x.cameraVectorDistance.CompareTo(y.cameraVectorDistance);
                return x.projectedDistance.CompareTo(y.projectedDistance);
            });



			return possibleJoints.ToArray ();
		}

        /// <summary>
        /// Try to position brick to fulfill given joint and check if this positions is valid.
        /// </summary>
        /// <returns><c>true</c>, if joint was checked, <c>false</c> otherwise.</returns>
        /// <param name="possibleJoint">Joint.</param>
        bool CheckJoint(PossibleJoint possibleJoint, bool snapToGrid)
		{
            //calculate position and rotation differences
            Vector3 positionDiff = 
                possibleJoint.projectedOtherJoint.joint.transform.position -
                possibleJoint.projectedBrickJoint.joint.transform.position;
            Quaternion rotationDiff = 
                Quaternion.Inverse (possibleJoint.projectedOtherJoint.joint.transform.rotation) *
                possibleJoint.projectedBrickJoint.joint.transform.rotation;

            //remember old position
            var oldPosition = brick.transform.position;
            var oldRotation = brick.transform.rotation;

            //set brick in position provided by joints
            brick.transform.position += positionDiff;
            brick.transform.rotation *= Quaternion.Inverse(rotationDiff);

            bool isValid = (!snapToGrid || brick.transform.position.y + lowestYoffset >= 0) && CheckPosition(possibleJoint);

            //if position is not valid, restore old postition
            if (!isValid)
            {
                brick.transform.position = oldPosition;
                brick.transform.rotation = oldRotation;
            }

            return isValid;
		}

        /// <summary>
        /// Check current brick position. Check if it is valid (all joints fits).
        /// </summary>
        /// <returns><c>true</c>, if position is valid, <c>false</c> otherwise.</returns>
        bool CheckPosition(PossibleJoint possibleJoint)
        {
            var brickJoints = possibleJoint.projectedBrickJoint.joint.parentBrick.joints;

            //Check free joints with another neighbour bricks 

            //get bounds
            AgaQBrick brick = possibleJoint.projectedBrickJoint.joint.parentBrick;
            var bounds = brick.GetBounds();
            //extend it by joint size
            bounds.Expand(jointColisionDistance * 2);
            //collide with other bricks
            var otherBricks = Physics.OverlapBox(bounds.center, bounds.extents, brick.transform.rotation, 1);

            //check joints at those other bricks
            foreach (var otherBrick in otherBricks)
            {
                if (otherBrick.gameObject == possibleJoint.projectedBrickJoint.joint.parentBrick.gameObject)
                    continue;
                
                var agaQBrick = otherBrick.GetComponent<AgaQBrick>();
                if (agaQBrick != null && !CheckJoints(brickJoints, agaQBrick.joints))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Check joints between two bricks.
        /// </summary>
        /// <returns><c>true</c>, if bricks can be joined in this position, <c>false</c> otherwise.</returns>
        /// <param name="brickJoints">Brick1 joints.</param>
        /// <param name="otherJoints">Brick2 joints.</param>
        bool CheckJoints(AgaQ.Bricks.Joints.Joint[] brickJoints, AgaQ.Bricks.Joints.Joint[] otherJoints)
        {
            foreach (var otherJoint in otherJoints)
            {
                //iterate over dragg brick joints
                foreach (var dragJoint in brickJoints)
                {
                    //compare joints
                    var distance = (dragJoint.transform.position - otherJoint.transform.position).magnitude;

                    //compare distance
                    float distanceScale = Mathf.Max(otherJoint.transform.lossyScale.x, dragJoint.transform.lossyScale.x);
                    if (distance > positionAccuracy && distance <= jointColisionDistance * distanceScale &&
                        !(dragJoint is FemaleBorderJoint) && !(otherJoint is FemaleBorderJoint))
                    {
                        DebugExtension.DebugArrow(otherJoint.transform.position,
                            dragJoint.transform.position, Color.yellow, 1);
                        return false;
                    }

                    //compare distance, typ of joints and rotations
                    if (distance <= positionAccuracy && 
                        (!AgaQ.Bricks.Joints.Joint.AreJoinable(dragJoint, otherJoint) ||
                         Quaternion.Angle(dragJoint.transform.rotation, otherJoint.transform.rotation) > rotationAccuracy))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get GameObjects of other bricks that has position behind or abowe current brick from camera perspective.
        /// </summary>
        /// <returns>The other brick's gameobjects.</returns>
        /// <param name="mousePosition">Mouse position.</param>
        GameObject[] GetOtherBricks(Vector3 mousePosition)
        {
            List<GameObject> bricks = new List<GameObject>();

            var cam = UnityEngine.Camera.main;

            var ray = cam.ScreenPointToRay(mousePosition);
            var mouseRayDirection = Quaternion.LookRotation(ray.direction);

            //calculate bounds dimensions
            Bounds bounds = brick.GetBounds();
            float minBrickDimension = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z) * 1.2f;
            bounds.extents = new Vector3(minBrickDimension, minBrickDimension, 1000f);
            bounds.center = brick.transform.position;// + dragPointOffset;

            //move bouds to position in front of the camera
            var distanceBricCamera = bounds.extents.z - (cam.transform.position - bounds.center).magnitude;
            bounds.center += ray.direction * distanceBricCamera;
//            DrawBounds(bounds, mouseRayDirection, Color.red);

            //check box collision with other bricks
            var colliders = Physics.OverlapBox(bounds.center, bounds.extents, mouseRayDirection , 1);

            foreach (var collider in colliders)
            {
                if (!bricks.Contains(collider.gameObject))
                    bricks.Add((collider.gameObject));
            }

            return bricks.ToArray();
        }
    }
}
