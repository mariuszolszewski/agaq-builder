using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System;

namespace AgaQ.Bricks
{
    /// <summary>
    /// Base abstact class with uuid.
    /// </summary>
    public abstract class Uuid : MonoBehaviour, ISerializationCallbackReceiver
    {
        /// <summary>
        /// Unique brick id. It is build with device id, create timestamp an some random number.
        /// </summary> 
        [SerializeField] Int64 _uuid;
        public Int64 uuid
        {
            get
            {
                if (_uuid <= 0)
                    CalculateUUID();

                return _uuid;
            }
        }

        public void OnBeforeSerialize()
        {
            if (_uuid == 0)
                CalculateUUID();
        }

        public void OnAfterDeserialize()
        {
        }
            
        /// <summary>
        /// Calcualte brick uuid.
        /// </summary>
        void CalculateUUID()
        {
            MD5 hasher = MD5.Create();
            var hashed = hasher.ComputeHash(Encoding.UTF8.GetBytes(SystemInfo.deviceUniqueIdentifier));
            int deviceId = Mathf.Abs(BitConverter.ToInt16(hashed, 0));
            int randomId = UnityEngine.Random.Range(0, Int16.MaxValue);
            Int32 timeId = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            _uuid = randomId * 281474976710656 + deviceId * 4294967296 + timeId;
        }
    }
}
