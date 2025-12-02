using TMPro;
using UnityEngine;

namespace GameContext.TicketHunt.View
{
    public class TicketHuntPopupRewardView : MonoBehaviour
    {
        [SerializeField]
        private bool _isGrandPrize;
        [Header("References")]
        [SerializeField]
        private GameObject _completedText;
        [SerializeField]
        private GameObject _progressBar;
        [SerializeField]
        private GameObject _lockedIcon;
        [SerializeField]
        private GameObject _lockedText;
        [SerializeField]
        private TicketProgressBarView _ticketProgressBarView;
        [SerializeField]
        private Transform _rewardParent;
        [Header("Regular Prize References")]
        [SerializeField]
        private GameObject _finishedStep;
        [SerializeField]
        private GameObject _stepCountBg;
        [SerializeField]
        private TextMeshProUGUI _stepCountText;
        
        public Transform RewardParent => _rewardParent;

        
        public void SetAsCompleted()
        {
            _completedText.SetActive(true);
            
            _lockedIcon.SetActive(false);
            _lockedText.SetActive(false);
            
            _progressBar.SetActive(false);

            if (!_isGrandPrize)
            {
                _finishedStep.SetActive(true); 
                _stepCountBg.SetActive(false);
                _stepCountText.gameObject.SetActive(false);
            }
        }
        
        public void SetAsInProgress(int currentTickets, int maxTickets, int currentStepId)
        {
            _completedText.SetActive(false);
            
            _lockedIcon.SetActive(false);
            _lockedText.SetActive(false);
            
            _progressBar.SetActive(true);
            _ticketProgressBarView.SetProgress(currentTickets, maxTickets);
            
            
            if (!_isGrandPrize)
            {
                _finishedStep.SetActive(false);
                _stepCountBg.SetActive(true);
                _stepCountText.gameObject.SetActive(true);
                _stepCountText.text = currentStepId.ToString();
            }
        }
        
        public void SetAsLocked(int currentStepId)
        {
            _completedText.SetActive(false);
            
            _lockedIcon.SetActive(true);
            _lockedText.SetActive(true);
            
            _progressBar.SetActive(false);
            
            if (!_isGrandPrize)
            {
                _finishedStep.SetActive(false);
                _stepCountBg.SetActive(true);
                _stepCountText.gameObject.SetActive(true);
                _stepCountText.text = currentStepId.ToString();
            }
        }
    }
}