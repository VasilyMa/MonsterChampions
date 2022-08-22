namespace Client
{
    struct DamagingEvent
    {
        public int UndergoEntity;
        public int DamagingEntity;
        public float DamageValue;

        /// <summary>
        /// Damage to Unit
        /// </summary>
        /// <param name="targetEntity"></param>
        /// <param name="damagingEntity"></param>
        /// <param name="damageValue"></param>
        public void Invoke(int targetEntity, int damagingEntity, float damageValue)
        {
            UndergoEntity = targetEntity;
            DamagingEntity = damagingEntity;
            DamageValue = damageValue;
        }

        /// <summary>
        /// Damage to Base
        /// </summary>
        /// <param name="targetEntity"></param>
        /// <param name="damagingEntity"></param>
        public void Invoke(int targetEntity, int damagingEntity)
        {
            UndergoEntity = targetEntity;
            DamagingEntity = damagingEntity;
        }
    }
}