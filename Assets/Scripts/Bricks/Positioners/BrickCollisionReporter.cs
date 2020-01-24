using UnityEngine;
using System.Collections.Generic;
using System;

namespace AgaQ.Bricks.Positioners
{
    public class BrickCollisionReporter : MonoBehaviour
    {
        [NonSerialized] public List<Brick> collidedBricks = new List<Brick>();
        [NonSerialized] public BoxCollider triggerCollider;

        void Start()
        {
            triggerCollider = gameObject.AddComponent<BoxCollider>();
            triggerCollider.isTrigger = true;
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        void OnTriggerEnter(Collider other)
        {
            var brick = other.gameObject.GetComponent<Brick>();
            if (brick is DragableBrick && !collidedBricks.Contains(brick))
                collidedBricks.Add(brick);
        }

        void OnTriggerExit(Collider other)
        {
            var brick = other.gameObject.GetComponent<Brick>();
            if (brick is DragableBrick)
                collidedBricks.Remove(brick);
        }
    }
}
