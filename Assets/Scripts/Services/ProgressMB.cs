using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client 
{
    public class ProgressMB : MonoBehaviour
    {
        private EcsWorld _world;
        private GameState _state;
        private float _currentAmonut;
        private float _maxAmount;
        [SerializeField] private Slider _slider;
        [SerializeField] private Text _ourHealthAmount;
        [SerializeField] private Text _enemyHealthAmount;
        public void Init(GameState state, EcsWorld world)
        {
            _world = world;
            _state = state; 
        }
        public void MaxSlider(int value)
        {
            _maxAmount = value;
            _currentAmonut = 0;
            _slider.value = _currentAmonut;
        }
        public void UpdateSlider(float ourHealth, float enemyHealth)
        {
            var allHealth = ourHealth + enemyHealth;
            _slider.value = ourHealth / allHealth;
        }
        public void UpdateHealth(int ourHealth, int enemyHealth)
        {
            _ourHealthAmount.text = ourHealth.ToString();
            _enemyHealthAmount.text = enemyHealth.ToString();
        }
    }
}

