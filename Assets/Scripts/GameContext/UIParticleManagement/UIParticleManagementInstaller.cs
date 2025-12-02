using UnityEngine;
using Zenject;

namespace GameContext.UIParticleManagement
{
    public class UIParticleManagementInstaller : MonoInstaller
    {
        [SerializeField]
        private Transform _poolParentTransform;
        [SerializeField]
        private string _targetedUIParticlePrefabResourcePath = "Prefabs/prefab_targeted_particles";
        
        public override void InstallBindings()
        {
            Container.BindMemoryPool<TargetedParticleView, TargetedParticleView.Pool>()
                .WithInitialSize(2)
                .FromComponentInNewPrefabResource(_targetedUIParticlePrefabResourcePath)
                .UnderTransform(_poolParentTransform);
            Container.BindInterfacesTo<UIParticleManager>().AsSingle();
        }
    }
}