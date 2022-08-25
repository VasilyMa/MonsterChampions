using UnityEngine;

namespace Client
{
    struct DropGoldEvent
    {
        public int GoldValue;
        public Vector3 DropPoint;

        /// <summary>
        /// Usually dropPoint = unit's death place
        /// </summary>
        /// <param name="goldValue"></param>
        /// <param name="dropPoint"></param>
        public void Invoke(int goldValue, Vector3 dropPoint)
        {
            GoldValue = goldValue;
            DropPoint = dropPoint;
        }
    }
}