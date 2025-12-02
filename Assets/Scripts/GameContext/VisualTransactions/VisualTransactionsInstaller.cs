using UnityEngine;
using Zenject;

namespace GameContext.VisualTransactions
{
    public class VisualTransactionsInstaller : MonoInstaller
    {
        [SerializeField]
        private VisualTransactionSettings _settings;
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<VisualTransactionManager>().AsSingle();
            Container.BindInstance(_settings).AsSingle();
        }
    }
}