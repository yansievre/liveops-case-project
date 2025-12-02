using UnityEngine;
using Zenject;

namespace GameContext
{
    public class MockGameplayInstaller : MonoInstaller
    {
        [SerializeField]
        private MockGameplaySettings _settings;
        
        public override void InstallBindings()
        {
            Container.Bind<MockGameplaySettings>().FromInstance(_settings).AsSingle();
            Container.BindInterfacesAndSelfTo<MockGameplayModel>().AsSingle().NonLazy();
        }
        
    }
}