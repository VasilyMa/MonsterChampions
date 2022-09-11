using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
namespace Client
{
    sealed class WinEventSystem : IEcsRunSystem
    {
        readonly EcsWorldInject _world = default;

        readonly EcsFilterInject<Inc<WinEvent>> _winEventFilter = default;

        readonly EcsFilterInject<Inc<UnitTag>, Exc<DeadTag, OnBoardUnitTag>> _aliveUnitsFilter = default;

        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;
        readonly EcsPoolInject<RewardComponentEvent> _rewardPool = default;

        readonly EcsPoolInject<DieEvent> _dieEventPool = default;

        readonly EcsSharedInject<GameState> _state = default;
        

        public void Run (IEcsSystems systems)
        {
            foreach (var eventEntity in _winEventFilter.Value)
            {
                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                interfaceComp.RewardPanelHolder.gameObject.SetActive(true);
                interfaceComp.HolderCards.transform.DOMove(interfaceComp.defaultPosCardHolder, 1f, false);
                interfaceComp.Progress.transform.GetChild(0).transform.DOMove(interfaceComp.defaultPosProgressHolder, 1f, false);
                interfaceComp.Resources.gameObject.SetActive(false);
                _rewardPool.Value.Add(_world.Value.NewEntity());
                Time.timeScale = 1;

                _state.Value.PreparedSystems = false;
                _state.Value.FightSystems = false;
                Debug.Log("Ты победил, умничка"); 
                int index = 0;
                if (SceneManager.GetActiveScene().buildIndex + 1 > SceneManager.sceneCountInBuildSettings - 1)
                {
                    index = 0;
                }
                else
                {
                    index = SceneManager.GetActiveScene().buildIndex + 1;
                }
                
                _state.Value.Settings.SceneNumber = index;
                if (_state.Value.Settings.Level % 3 == 0)
                {
                    if (_state.Value.Settings.MaxLevelRewardedCard < _state.Value._monsterStorage.monster.Length - 1)
                        _state.Value.Settings.MaxLevelRewardedCard++;
                }
                _state.Value.Settings.Level++;
                KillAllUnits();
                _state.Value.SaveGameSetting();
                _winEventFilter.Pools.Inc1.Del(eventEntity);
            }
        }

        private void KillAllUnits()
        {
            foreach (var unitEntity in _aliveUnitsFilter.Value)
            {
                var dieEventEntity = _world.Value.NewEntity();

                _dieEventPool.Value.Add(dieEventEntity).Invoke(unitEntity);
            }
        }
    }
}