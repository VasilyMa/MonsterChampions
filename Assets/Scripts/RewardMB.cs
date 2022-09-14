using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.EcsLite;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Client
{
    public class RewardMB : MonoBehaviour
    {
        public Transform vfx;
        public Transform card;
        private EcsWorld _world;
        private GameState _state;
        private EcsPool<InterfaceComponent> _interfacePool;
        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
            _interfacePool = _world.GetPool<InterfaceComponent>();

            card = transform.GetChild(1).transform;
            vfx = transform.GetChild(0).transform;
        }
        public void OpenNewCard()
        {
            var button = GameObject.Find("ButtonNext");
            button.GetComponent<Button>().enabled = false;
            transform.GetComponent<Image>().raycastTarget = false;
            var cardInfo = card.GetComponent<CardInfo>();
            cardInfo.UpdateCardInfo();
            card.gameObject.SetActive(true);
            vfx.transform.GetChild(2).gameObject.SetActive(true);
            card.GetComponent<Image>().raycastTarget = false;
            var Sequence = DOTween.Sequence();
            
            Sequence.Append(vfx.transform.DOLocalRotate(new Vector3(0, -90, 0), 0.5f));
            Sequence.Append(card.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f)).OnComplete(()=>StartCoroutine(WaitBiomLevel()));
        }
        private IEnumerator WaitBiomLevel()
        {
            yield return new WaitForSeconds(1.5f);
            OpenBiomPanel();
        }
        private void OpenBiomPanel()
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            interfaceComp.BiomHolder.gameObject.SetActive(true);
            interfaceComp.RewardPanelHolder.gameObject.SetActive(false);
            StartCoroutine(WaitNextLevel());
        }
        private IEnumerator WaitNextLevel()
        {
            yield return new WaitForSeconds(1.5f);
            NextLevel();
        }


        void NextLevel()
        {
            SceneManager.LoadScene(_state.Settings.SceneNumber);
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            interfaceComp.RewardPanelHolder.gameObject.SetActive(false);
            interfaceComp.HolderCards.gameObject.SetActive(false);
            interfaceComp.MenuHolder.gameObject.SetActive(true);
        }
    }
}
