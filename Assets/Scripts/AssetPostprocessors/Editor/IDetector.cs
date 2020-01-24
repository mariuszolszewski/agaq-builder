using UnityEngine;

namespace AssetPostprocessors
{
    public interface IDetector
    {
        Pin[] DetectPins(Mesh mesh);
    }
}
