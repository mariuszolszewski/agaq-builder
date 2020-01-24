using UnityEngine;
namespace AgaQ.Camera.Tools
{
    /// <summary>
    /// Camera tool interface
    /// </summary>
    public interface ITool
    {
        void Start(CameraController cameraController);
        void Update(CameraController cameraController);
    }
}
