using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
namespace Client
{
    sealed class LoseEventSystem : IEcsRunSystem
    {
        readonly EcsWorldInject _world = default;

        readonly EcsFilterInject<Inc<LoseEvent>> _loseEventFilter = default;

        readonly EcsFilterInject<Inc<UnitTag>, Exc<DeadTag, OnBoardUnitTag>> _aliveUnitsFilter = default;

        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;

        readonly EcsPoolInject<DieEvent> _dieEventPool = default;

        readonly EcsSharedInject<GameState> _state = default;
        public void Run (IEcsSystems systems)
        {
            foreach (var eventEntity in _loseEventFilter.Value)
            {
                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                interfaceComp.Resources.gameObject.SetActive(false);
                interfaceComp.LoseHolder.gameObject.SetActive(true);
                interfaceComp.LoseHolder.DOMove(GameObject.Find("TargetLoseWin").transform.position, 1f, false);
                interfaceComp.HolderCards.transform.DOMove(interfaceComp.defaultPosCardHolder, 1f, false);
                interfaceComp.Progress.transform.GetChild(0).transform.DOMove(interfaceComp.defaultPosProgressHolder, 1f, false);

                KillAllUnits();

                _state.Value.PreparedSystems = false;
                _state.Value.FightSystems = false;
                Debug.Log("Ты всрал, дружок-пирожок");
                _loseEventFilter.Pools.Inc1.Del(eventEntity);
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