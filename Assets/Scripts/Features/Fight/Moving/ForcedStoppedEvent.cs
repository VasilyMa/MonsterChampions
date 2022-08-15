namespace Client
{
    struct ForcedStoppedEvent
    {
        public int StoppedEntity;

        public void Invoke(int stoppedEntity)
        {
            StoppedEntity = stoppedEntity;
        }
    }
}