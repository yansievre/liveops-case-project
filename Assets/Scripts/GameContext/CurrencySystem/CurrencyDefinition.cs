using UnityEngine;

namespace GameContext.CurrencySystem
{
    [CreateAssetMenu(fileName = "CurrencyDefinition", menuName = "Game/Currency Definition", order = 0)]
    public class CurrencyDefinition : ScriptableObject
    {
        [SerializeField]
        private string _uniqueIdentifier;

        // In a real setting, this would be a localization key
        [SerializeField]
        private string _visibleName;

        [SerializeField]
        private string _prefix;

        [SerializeField]
        private string _suffix;

        [SerializeField]
        private Sprite _icon;
        [SerializeField]
        private Material _uiParticleMaterial;

        public string UniqueIdentifier => _uniqueIdentifier;

        public string VisibleName => _visibleName;

        public Sprite Icon => _icon;
        public string Prefix => _prefix;
        public string Suffix => _suffix;
        public Material UiParticleMaterial => _uiParticleMaterial;
    }
}