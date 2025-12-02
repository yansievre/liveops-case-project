using System;
using GameContext.TicketHunt.DataLoader;
using R3;
using UnityEngine;
using Zenject;

namespace GameContext.TicketHunt
{
    public class TicketHuntHUDSpawner : MonoBehaviour
    {
        //Will only be injected if the factory exists, the factory should only exist if an event exists.
        [InjectOptional]
        private TicketHuntHUDView.Factory _ticketHuntHUDViewFactory;
        [InjectOptional]
        private TicketHuntModel _model;

        private TicketHuntHUDView _currentView;
        private void Start()
        {            
            if(_model == null)
            {
                Destroy(this);
                return;
            }
            
            if(DetermineEventVisibility(_model.EventLive.CurrentValue, _model.IsCompleted.CurrentValue))
            {
                if (_currentView == null && _ticketHuntHUDViewFactory != null)
                {
                    _currentView = _ticketHuntHUDViewFactory.Create();
                    _currentView.transform.SetParent(transform, false);
                }
            }
        }
        
        private bool DetermineEventVisibility(bool isLive, bool isCompleted)
        {
            return isLive && !isCompleted;
        }
    }
}