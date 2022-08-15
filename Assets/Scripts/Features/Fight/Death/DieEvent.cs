namespace Client
{
    struct DieEvent
    {
        public int DyingEntity;

        /// <summary>
        /// Write dying entity for DieEventSystem
        /// </summary>
        /// <param name="dyingEntity"></param>
        public void Invoke(int dyingEntity)
        {
            DyingEntity = dyingEntity;
        }
    }
}