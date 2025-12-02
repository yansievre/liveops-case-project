using GlobalContext.Persistence;
using ProjectContext;
using ProjectContext.DateTimeSource;
using UnityEngine;
using Zenject;

#region Installed Classes
using ActiveGameStateHandler = GlobalContext.Persistence.PlayerPrefsGameStateHandler;
using ActiveDateTimeSource = ProjectContext.DateTimeSource.RoundedDownDateTimeSource;
using ActiveLiveOpsLoader = GameContext.TicketHunt.DataLoader.LiveOpsResourcesLoader;
#endregion

namespace GlobalContext
{
    public class GlobalContextInstaller : MonoInstaller
    {
        [SerializeField]
        private LoadingScreenHandle _loadingScreenHandle;
        public override void InstallBindings()
        {
            Container.Bind<ILoadingScreenHandle>().FromInstance(_loadingScreenHandle).AsSingle();
            Container.Bind<IGameStateHandler>().To<ActiveGameStateHandler>().AsSingle();
            Container.Bind<SaveOnExit>().ToSelf().AsSingle().NonLazy();
            Container.BindInterfacesTo<TimeManager>().AsSingle();
            Container.BindInterfacesTo<GameLoader>().AsSingle();
            Container.Bind<IDateTimeSource>().To<ActiveDateTimeSource>().AsSingle();

            #region LiveOps
            Container.BindInterfacesTo<ActiveLiveOpsLoader>().AsSingle();
            #endregion
            
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Container.Decorate<IDateTimeSource>().With<DateTimeSourceOffsetDecorator>();
            Container.BindInterfacesAndSelfTo<DateTimeOffset>().AsSingle();
#endif
            
            _loadingScreenHandle.Show();
        }
    }
}