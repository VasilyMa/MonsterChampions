namespace Client
{
    struct DieEvent
    {
        public int DyingEntity;
        public bool WithoutGold;

        /// <summary>
        /// Write dying entity for DieEventSystem
        /// </summary>
        /// <param name="dyingEntity"></param>
        public void Invoke(int dyingEntity, bool withoutGold = false)
        {
            DyingEntity = dyingEntity;
            WithoutGold = withoutGold;
        }
    }
}