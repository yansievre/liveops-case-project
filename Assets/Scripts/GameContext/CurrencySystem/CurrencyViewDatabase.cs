using System.Collections.Generic;
using GameContext.CurrencySystem.Views;
using UnityEngine;

namespace GameContext.CurrencySystem
{
    public enum CurrencyViewContext
    {
        None,
        TopBar,
        EventHud
    }
    
    public class CurrencyViewDatabase : ICurrencyViewDatabase
    {
        private class CurrencyViewEntry
        {
            public CurrencyViewContext context;
            public ICurrencyView view;
        }
        private readonly Dictionary<string, List<CurrencyViewEntry>> _viewsByCurrencyId = new Dictionary<string, List<CurrencyViewEntry>>();
        
        public void RegisterView(ICurrencyView view, CurrencyViewContext context = CurrencyViewContext.None)
        {
            if (!_viewsByCurrencyId.ContainsKey(view.CurrencyId))
                _viewsByCurrencyId[view.CurrencyId] = new List<CurrencyViewEntry>();
            var entry = new CurrencyViewEntry
            {
                context = context,
                view = view
            };
            _viewsByCurrencyId[view.CurrencyId].Add(entry);
        }
        
        public void UnregisterView(ICurrencyView view)
        {
            if (!_viewsByCurrencyId.TryGetValue(view.CurrencyId, out var value))
                return;
            value.RemoveAll(e => e.view == view);
            if (_viewsByCurrencyId[view.CurrencyId].Count == 0)
                _viewsByCurrencyId.Remove(view.CurrencyId);
        }

        public ICurrencyView GetViewForCurrency(string currencyId, CurrencyViewContext context = CurrencyViewContext.None)
        {
            if (!_viewsByCurrencyId.TryGetValue(currencyId, out var entries))
                return null;
            foreach (var entry in entries)
            {
                if (entry.context == context || context == CurrencyViewContext.None)
                    return entry.view;
            }
            Debug.LogError($"No view found for currency {currencyId} in context {context}");
            return null;
        }

    }
}