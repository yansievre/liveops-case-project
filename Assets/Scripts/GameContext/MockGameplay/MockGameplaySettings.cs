using GameContext.CurrencySystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameContext
{
    [CreateAssetMenu(fileName = "MockGameplaySettings", menuName = "Game/MockGameplaySettings", order = 1)]
    public class MockGameplaySettings : ScriptableObject
    {
        [SerializeField]
        private Transaction _simulateGameplayCost;

        [HorizontalGroup("Reward")]
        [SerializeField]
        private string _simulateGameplayRewardCurrency;
        [HorizontalGroup("Reward")]
        [SerializeField]
        private Vector2Int _simulateGameplayRewardRange;
        
        [SerializeField]
        private int _simulateGameplayCooldownSeconds = 5;
        
        public Transaction SimulateGameplayCost => _simulateGameplayCost;
        public string SimulateGameplayRewardCurrency => _simulateGameplayRewardCurrency;
        public Vector2Int SimulateGameplayRewardRange => _simulateGameplayRewardRange;
        public int SimulateGameplayCooldownSeconds => _simulateGameplayCooldownSeconds;
    }
}