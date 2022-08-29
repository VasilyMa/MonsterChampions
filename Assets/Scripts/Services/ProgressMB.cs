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
        private float _currentAmonut = 0;
        private float _maxAmount;
        [SerializeField] private Slider _slider;
        [SerializeField] private Text _ourHealthAmount;
        [SerializeField] private Text _enemyHealthAmount;
        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
        }
        public void UpdateHealth(float ourHealth, float enemyHealth)
        {
            _ourHealthAmount.text = ourHealth.ToString();
            _enemyHealthAmount.text = enemyHealth.ToString();
            var allHealth = ourHealth + enemyHealth;
            _currentAmonut = ourHealth / allHealth;
            _slider.value = _currentAmonut;
        }
    }
}

