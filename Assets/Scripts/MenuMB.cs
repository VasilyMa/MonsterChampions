using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Client
{
    public class MenuMB : MonoBehaviour
    {
        private EcsWorld _world;
        private GameState _state;
        private EcsPool<InterfaceComponent> _interfacePool;
        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
            _interfacePool = _world.GetPool<InterfaceComponent>();
        }
        public void Play()
        {
            SceneManager.LoadScene(_state.CurrentLevel);
        }
        public void ToCollection()
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            UpdateCollection();
            interfaceComp.MenuHolder.gameObject.SetActive(false);
            interfaceComp.CollectionMenu.gameObject.SetActive(true);

        }
        public void UpdateCollection()
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            if (_state.Collection.CollectionUnits.Count > 0)
            {
                foreach (var card in _state.Collection.CollectionUnits)
                {
                    var addedCard = (GameObject)GameObject.Instantiate(Resources.Load("CollectionCard"), interfaceComp.CollectionHolder);
                    var cardInfo = addedCard.GetComponent<CardInfo>();
                    cardInfo.unitID = card.UnitID;
                    cardInfo.NameUnit = card.NameUnit;
                    cardInfo.Health = card.Health;
                    cardInfo.Damage = card.Damage;
                    cardInfo.Elemental = card.Elemental;
                    cardInfo.MoveSpeed = card.MoveSpeed;
                    cardInfo.Prefabs = card.Prefabs;
                    cardInfo.UpdateCardInfo();
                }
            }
        }
        public void Exit()
        {
            Application.Quit();
        }
    }
}

