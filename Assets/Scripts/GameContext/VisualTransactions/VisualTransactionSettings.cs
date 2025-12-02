using UnityEngine;

namespace GameContext.VisualTransactions
{
    [CreateAssetMenu(fileName = "VisualTransactionSettings", menuName = "Game/VisualTransactions/VisualTransactionSettings", order = 1)]
    public class VisualTransactionSettings : ScriptableObject
    {
        [SerializeField, Min(1)]
        private int _maxBurstCount = 5;
        [SerializeField, Min(1)]
        private int _maxParticlesPerBurst = 6;
        [SerializeField, Min(0.01f)]
        private float _burstInterval = 0.1f;
        
        public int MaxBurstCount => _maxBurstCount;
        public int MaxParticlesPerBurst => _maxParticlesPerBurst;
        public float BurstInterval => _burstInterval;
    }
}