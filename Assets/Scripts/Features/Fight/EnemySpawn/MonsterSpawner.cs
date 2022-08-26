using System.Collections.Generic;

namespace Client
{
    struct MonsterSpawner
    {
        public float TimerMaxValue;
        public float TimerCurrentValue;
        public int MonsterLevel;
        public List<MonsterStorage> MonsterStorage;
    }
}