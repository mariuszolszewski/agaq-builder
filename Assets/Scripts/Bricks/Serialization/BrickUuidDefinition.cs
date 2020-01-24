using System;

namespace AgaQ.Bricks.Serialization
{
    /// <summary>
    /// Structure descibes where to search brick recources with given uuid
    /// </summary>
    [Serializable]
    public struct BrickUuidDefinition
    {
        public Int64 brickUuid;
        public string resourcePath;

        public BrickUuidDefinition(Int64 uuid, string path)
        {
            brickUuid = uuid;
            resourcePath = path;
        }
    }
}
