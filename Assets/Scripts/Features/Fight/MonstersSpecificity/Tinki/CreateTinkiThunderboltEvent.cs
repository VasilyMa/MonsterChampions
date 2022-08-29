namespace Client
{
    struct CreateTinkiThunderboltEvent
    {
        public int TinkiEntity;
        public int TargetEntity;

        /// <summary>
        /// Create thunderbolt from Tinki to hes target
        /// </summary>
        /// <param name="tinkiEntity"></param>
        /// <param name="targetEntity"></param>
        public void Invoke(int tinkiEntity, int targetEntity)
        {
            TinkiEntity = tinkiEntity;
            TargetEntity = targetEntity;
        }
    }
}