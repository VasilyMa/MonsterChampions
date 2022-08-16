using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
namespace Client {
    sealed class InitInterface : MonoBehaviour, IEcsInitSystem  
    {
        readonly EcsWorldInject _world = default;

        readonly EcsSharedInject<GameState> _state = default;

        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;

        public void Init(IEcsSystems systems)
        {
            var entity = _world.Value.NewEntity();
            _state.Value.InterfaceEntity = entity;
            ref var interfaceComp = ref _interfacePool.Value.Add(entity);
            interfaceComp.MainCanvas = FindObjectOfType<Canvas>();
            interfaceComp.BuyCard = FindObjectOfType<BuyCardMB>();
            interfaceComp.HolderCards = interfaceComp.BuyCard.transform;
            interfaceComp.BuyCard.Init(systems.GetWorld(), systems.GetShared<GameState>());

            interfaceComp.CollectionManager = FindObjectOfType<CollectionMB>();
            interfaceComp.CollectionHolder = interfaceComp.CollectionManager.transform.GetChild(1).transform;
            interfaceComp.DeckHolder = interfaceComp.CollectionManager.transform.GetChild(0).transform;
            interfaceComp.CollectionCards = new System.Collections.Generic.List<GameObject>();
            interfaceComp.DeckCards = new System.Collections.Generic.List<GameObject>();
            interfaceComp.CollectionManager.Init(systems.GetWorld(), systems.GetShared<GameState>());

            foreach (var card in _state.Value.Collection.CollectionUnits)
            {
                var addedCard = (GameObject)GameObject.Instantiate(Resources.Load("CollectionCard"), interfaceComp.CollectionHolder);
                var cardInfo = addedCard.GetComponent<CardInfo>();
                cardInfo.unitID = card.UnitID;
            }

            interfaceComp.CollectionHolder.gameObject.SetActive(false);
            interfaceComp.HolderCards.gameObject.SetActive(false);
            interfaceComp.DeckHolder.gameObject.SetActive(false);
        }
    }
}