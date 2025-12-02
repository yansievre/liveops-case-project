using GameContext.TicketHunt.View;
using UnityEngine;
using Zenject;

namespace GameContext.TicketHunt
{
    [CreateAssetMenu(fileName = "TicketHuntInstaller", menuName = "Installers/TicketHuntInstaller")]
    public class TicketHuntInstaller : ScriptableObjectInstaller<TicketHuntInstaller>
    {
        [SerializeField]
        private string _ticketHuntHUDViewPrefabAddress = "Prefabs/prefab_ticket_hunt_hud";
        [SerializeField]
        private string _ticketHuntPopupViewPrefabAddress = "Prefabs/prefab_ticket_hunt_popup";
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<TicketHuntModel>().AsSingle();
            
            //Ideally this would be bound to an abstract loading interface, but for simplicity, we bind directly to resources here.
            Container.BindFactory<TicketHuntHUDView, TicketHuntHUDView.Factory>().FromComponentInNewPrefabResource(_ticketHuntHUDViewPrefabAddress).AsSingle();
            Container.BindFactory<TicketHuntPopupView, TicketHuntPopupView.Factory>().FromComponentInNewPrefabResource(_ticketHuntPopupViewPrefabAddress).AsSingle();
        }
    }
    
}