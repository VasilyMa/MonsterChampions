using System.Collections.Generic;

namespace Client
{
    struct MonsterSpawner
    {
        public BaseTagMB MonsterSpawnerInfo;
        public int ActualSquad;

        public float TimerMaxValue; // to do ay del this
        public float TimerCurrentValue; // to do ay del this
        public int MonsterLevel; // to do ay del this
        public List<MonsterStorage> MonsterStorage; // to do ay del this
    }
}