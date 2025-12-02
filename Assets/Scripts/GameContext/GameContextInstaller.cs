using GameContext.PopupManagement;
using GameContext.TicketHunt;
using GameContext.TicketHunt.DataLoader;
using UnityEngine;
using Zenject;

namespace GameContext
{
    public class GameContextInstaller : MonoInstaller
    {
        [SerializeField]
        private TicketHuntInstaller _ticketHuntInstaller;
        public override void InstallBindings()
        {
            var liveOpsLoader = Container.Resolve<ILiveOpsLoader>();
            if (liveOpsLoader.TicketHuntData.CurrentValue != null)
            {
                Container.Inject(_ticketHuntInstaller);
                _ticketHuntInstaller.InstallBindings();
            }

            Container.BindInterfacesTo<PopupManager>().AsSingle().NonLazy();
        }
    }
}