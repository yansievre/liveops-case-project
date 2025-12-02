using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace GameContext.CurrencySystem
{
    [CreateAssetMenu(fileName = "CurrencySystemInstaller", menuName = "Installers/Currency System Installer", order = 0)]
    public class CurrencySystemInstaller : ScriptableObjectInstaller
    {
        [SerializeField, Required]
        private CurrencyDefinitions _currencyDefinitions;
        public override void InstallBindings()
        {
            if (_currencyDefinitions == null || _currencyDefinitions.CurrencyDefinitionList.Count == 0)
            {
                throw new Exception("No currency definitions provided to CurrencySystemInstaller");
            }
            if (!_currencyDefinitions.AreIdentifiersUnique)
            {
                throw new Exception("Currency identifiers must be unique");
            }

            Container.Bind<CurrencyDefinitions>().FromInstance(_currencyDefinitions).AsSingle();
            Container.BindInterfacesTo<CurrencyManager>().AsSingle();
            Container.Bind<ICurrencyViewDatabase>().To<CurrencyViewDatabase>().AsSingle();
            
            Container.BindExecutionOrder<CurrencyManager>(-100);
            
        }
        

    }
}