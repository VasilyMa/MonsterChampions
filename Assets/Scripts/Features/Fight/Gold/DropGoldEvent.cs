using UnityEngine;

namespace Client
{
    struct DropGoldEvent
    {
        public int DropGoldEntity;

        public void Invoke(int dropGoldEntity)
        {
            DropGoldEntity = dropGoldEntity;
        }
    }
}