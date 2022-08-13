namespace Client
{
    struct DamagingEvent
    {
        public int DamageEntity;
        public int WhoDoDamageEntity;
        public float DamageValue;
        
        /// <summary>
        /// Damage to Base
        /// </summary>
        /// <param name="damageEntity"></param>
        /// <param name="whoDoDamageEntity"></param>
        public void Invoke(int damageEntity, int whoDoDamageEntity)
        {
            DamageEntity = damageEntity;
            WhoDoDamageEntity = whoDoDamageEntity;
        }

        /// <summary>
        /// Damage to Unit
        /// </summary>
        /// <param name="damageEntity"></param>
        /// <param name="whoDoDamageEntity"></param>
        /// <param name="damageValue"></param>
        public void Invoke(int damageEntity, int whoDoDamageEntity, float damageValue)
        {
            DamageEntity = damageEntity;
            WhoDoDamageEntity = whoDoDamageEntity;
            DamageValue = damageValue;
        }
    }
}