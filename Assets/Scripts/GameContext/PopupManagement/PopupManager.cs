using GameContext.TicketHunt.View;
using UnityEngine;
using Zenject;

namespace GameContext.PopupManagement
{
    public class PopupManager : IInitializable, IPopupManager
    {
        private readonly TicketHuntPopupView.Factory _ticketHuntPopupViewFactory;
        private readonly Transform _popupRoot;

        #region Single Popups

        private TicketHuntPopupView _ticketHuntPopupView;
        private GameObject _gameCanvas;

        #endregion

        public PopupManager(
            [InjectOptional] TicketHuntPopupView.Factory ticketHuntPopupViewFactory)
        {
            _ticketHuntPopupViewFactory = ticketHuntPopupViewFactory;
            _popupRoot = GameObject.FindGameObjectWithTag("PopupParent").transform;
            _gameCanvas = GameObject.FindGameObjectWithTag("GameCanvas");
        }

        public void Initialize()
        {
            if (_ticketHuntPopupViewFactory != null)
            {
                _ticketHuntPopupView = _ticketHuntPopupViewFactory.Create();
                _ticketHuntPopupView.gameObject.SetActive(false);
                _ticketHuntPopupView.transform.SetParent(_popupRoot, false);
            }
        }
        
        public void ShowTicketHuntPopup()
        {
            if (_ticketHuntPopupView == null)
            {
                Debug.LogWarning("TicketHuntPopupView Factory is not available. Cannot show Ticket Hunt Popup.");
                return;
            }

            
            _ticketHuntPopupView.gameObject.SetActive(true);
            _gameCanvas.SetActive(false);
        }
        
        public void HideTicketHuntPopup()
        {
            if (_ticketHuntPopupView == null)
            {
                Debug.LogWarning("TicketHuntPopupView Factory is not available. Cannot hide Ticket Hunt Popup.");
                return;
            }

            _ticketHuntPopupView.gameObject.SetActive(false);
            _gameCanvas.SetActive(true);
        }
    }
}