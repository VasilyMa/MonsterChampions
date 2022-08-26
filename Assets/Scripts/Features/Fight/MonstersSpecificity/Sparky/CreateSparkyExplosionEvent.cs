namespace Client
{
    struct CreateSparkyExplosionEvent
    {
        public int SparkyEntity;

        /// <summary>
        /// Invoke explosion beside this Sparky
        /// </summary>
        /// <param name="sparkyEntity"></param>
        public void Invoke(int sparkyEntity)
        {
            SparkyEntity = sparkyEntity;
        }
    }
}