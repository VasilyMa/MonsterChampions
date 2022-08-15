namespace Client
{
    struct DamagingEvent
    {
        public int UndergoEntity;
        public int WhoDoDamageEntity;
        public float DamageValue;
        
        /// <summary>
        /// Damage to Base
        /// </summary>
        /// <param name="damageEntity"></param>
        /// <param name="whoDoDamageEntity"></param>
        public void Invoke(int damageEntity, int whoDoDamageEntity)
        {
            UndergoEntity = damageEntity;
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
            UndergoEntity = damageEntity;
            WhoDoDamageEntity = whoDoDamageEntity;
            DamageValue = damageValue;
        }
    }
}