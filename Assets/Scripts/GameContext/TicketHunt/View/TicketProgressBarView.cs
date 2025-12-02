using TMPro;
using UnityEngine;

namespace GameContext.TicketHunt.View
{
    public class TicketProgressBarView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private TextMeshProUGUI _progressText;
        [SerializeField]
        private RectTransform _progressFillRect;
        [SerializeField]
        private Vector2 _fillMinMaxAnchors = new Vector2(0, 1);
        
        public void SetProgress(int current, int max)
        {
            var progress = Mathf.Clamp01((float)current / max);
            _progressFillRect.anchorMax = new Vector2(Mathf.Lerp(_fillMinMaxAnchors.x, _fillMinMaxAnchors.y, progress), _progressFillRect.anchorMax.y);
            _progressText.text = $"{current}/{max}";
        }
    }
}