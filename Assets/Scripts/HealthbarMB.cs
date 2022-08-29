using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;


namespace Client 
{
    public class HealthbarMB : MonoBehaviour
    {
        private EcsWorldInject _world;
        private GameState _state;
        [SerializeField] private Slider _slider;
        [SerializeField] private Gradient _gradient;
        [SerializeField] private Image _image;
        [SerializeField] private float _curHp;
        [SerializeField] private float _maxHP;
        [SerializeField] private GameObject _healthBar;
        [SerializeField] private Text _amount;
        private EcsPool<CameraComponent> _cameraPool = null;
        public void Init(EcsWorldInject world, GameState state)
        {
            _world = world;
            _state = state;
            _cameraPool = _world.Value.GetPool<CameraComponent>();
        }
        public void SetMaxHealth(float health)
        {
            _slider.maxValue = health;
            _slider.value = health;
            _maxHP = health;
            _amount.text = health.ToString();
            _image.color = _gradient.Evaluate(1f);
        }
        public void SetHealth(float health)
        {
            _slider.value = health;
            _curHp = health;
            _amount.text = health.ToString();
        }
        public void UpdateHealth(float currentHP)
        {
            _curHp = currentHP;
            _slider.value = _curHp;
            _image.color = _gradient.Evaluate(_slider.normalizedValue);
            _amount.text = _curHp.ToString();
            if (_slider.value <= 0)
                _healthBar.SetActive(false);
        }
        private void CameraFollow()
        {
            ref var cameraComp = ref _cameraPool.Get(_state.CameraEntity);
            _healthBar.transform.LookAt(_healthBar.transform.position + cameraComp.CameraTransform.forward);
        }
        public void ToggleSwitcher()
        {
            _healthBar.SetActive(!_healthBar.activeSelf);
        }
        public void Disable()
        {
            if (_healthBar.activeSelf) _healthBar.SetActive(false);
        }
        public void Enableble()
        {
            if (!_healthBar.activeSelf) _healthBar.SetActive(true);
        }
        private void Update()
        {
            
            if (_curHp == _maxHP || _curHp <= 0)
                _healthBar.SetActive(false);
            else
            {
                CameraFollow(); 
                _healthBar.SetActive(true);
            }
        }
    }
}
