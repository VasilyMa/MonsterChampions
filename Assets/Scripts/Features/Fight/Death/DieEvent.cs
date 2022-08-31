namespace Client
{
    struct DieEvent
    {
        public int DyingEntity;
        public bool IsTouchedBase;

        /// <summary>
        /// Write dying entity for DieEventSystem
        /// </summary>
        /// <param name="dyingEntity"></param>
        public void Invoke(int dyingEntity, bool isTouchedBase = false)
        {
            DyingEntity = dyingEntity;
            IsTouchedBase = isTouchedBase;
        }
    }
}