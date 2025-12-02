using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameContext.CurrencySystem
{
    [CreateAssetMenu(fileName = "CurrencyDefinitions", menuName = "Game/Currency Definitions", order = 0)]
    public class CurrencyDefinitions : ScriptableObject
    {
        [SerializeField, ValidateInput(nameof(IdentifiersUnique), "Currency identifiers must be unique")]
        private List<CurrencyDefinition> _currencyDefinitions;
        
        public List<CurrencyDefinition> CurrencyDefinitionList => _currencyDefinitions;
        
        public bool AreIdentifiersUnique => IdentifiersUnique(_currencyDefinitions);
        private bool IdentifiersUnique(List<CurrencyDefinition> value)
        {
            return value.Select(x => x.UniqueIdentifier).Distinct().Count() == value.Count;
        }
    }
}