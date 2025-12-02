using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameContext.CurrencySystem.Views
{
    public class BasicCurrencyView : MonoBehaviour
    {
        public TextMeshProUGUI amountText;
        public Image iconImage;

        public bool HasIcon => iconImage != null;
        public bool HasAmount => amountText != null;
        
        
        public void SetAmount(string amount)
        {
            if (amountText != null)
                amountText.text = amount;
        }
        
        public void SetIcon(Sprite icon)
        {
            if (iconImage != null)
                iconImage.sprite = icon;
        }
    }
}