namespace Client
{
    struct RefreshHealthBarEvent
    {
        public int UnitEntity;

        /// <summary>
        /// For who refresh healthBar
        /// </summary>
        /// <param name="unitEntity"></param>
        public void Invoke(int unitEntity)
        {
            UnitEntity = unitEntity;
        }
    }
}