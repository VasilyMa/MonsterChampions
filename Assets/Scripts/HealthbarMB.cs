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
        [SerializeField] private Image border_01;
        [SerializeField] private Image border_02;

        [Header("ShieldInfo")]
        [SerializeField] private Slider _sliderShield;
        [SerializeField] private Image _imageShield;
        [SerializeField] private float _durabilityCurrent;
        [SerializeField] private float _durabilityMax;

        private EcsPool<CameraComponent> _cameraPool = null;
        public void Init(EcsWorldInject world, GameState state)
        {
            _world = world;
            _state = state;
            _cameraPool = _world.Value.GetPool<CameraComponent>();
            _sliderShield.gameObject.SetActive(false);
            
        }
        public void SetMaxShield(float amount)
        {
            _sliderShield.gameObject.SetActive(true);
            _sliderShield.maxValue = amount;
            _sliderShield.value = amount;
        }
        public void ShieldUpdate(float value)
        {
            _durabilityCurrent = value;
            _sliderShield.value = _durabilityCurrent;
            if (_sliderShield.value <= 0)
            {
                _sliderShield.gameObject.SetActive(false);
            }
        }

        public void SetMaxHealth(float health)
        {
            _slider.maxValue = health;
            _slider.value = health;
            _maxHP = health;
            _curHp = health;
            _amount.text = health.ToString();
            _image.color = _gradient.Evaluate(1f); 
            var parent = transform.parent.GetComponent<UnitTagMB>();
            border_01.color = Color.red;
            border_02.color = Color.red;
            if (parent.IsFriendly)
            {
                border_01.color = Color.white;
                border_02.color = Color.white;
            }
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
            if (/*_curHp == _maxHP || */_curHp <= 0)
                _healthBar.SetActive(false);
            else
            {
                CameraFollow();
                _healthBar.SetActive(true);
            }
        }
    }
}
