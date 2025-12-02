using GameContext.CurrencySystem;
using GameContext.UIParticleManagement;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace GameContext
{
    public class Cheats : MonoBehaviour
    {
        [Inject]
        private ICurrencyManager _currencyManager;
        [Inject]
        private IUIParticleManager _iuiParticleManager;
        
        public TargetedUIParticlePlayInfo testParticleInfo;
        
        [Button]
        public void ChangeMoney(string id, long amount)
        {
            var handle = _currencyManager.GetCurrencyHandle(id);
            if( amount  > 0)
                handle.Add(amount);
            else
                handle.Remove(-amount);
        }
        
        [Button]
        public void PlayTestParticles()
        {
            _iuiParticleManager.PlayTargetedParticles(testParticleInfo);
        }
    }
}