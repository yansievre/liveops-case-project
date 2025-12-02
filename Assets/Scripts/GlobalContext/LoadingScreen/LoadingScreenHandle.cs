using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectContext
{
    public class LoadingScreenHandle : MonoBehaviour, ILoadingScreenHandle
    {
        [SerializeField]
        private Slider _slider;
        [SerializeField]
        private TextMeshProUGUI _message;
        
        public void Show()
        {
            gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        public void SetProgress(float progress, string msg)
        {
            _message.text = msg;
            _slider.value = progress;
        }
    }
}