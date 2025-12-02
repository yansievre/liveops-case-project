using GameContext.CurrencySystem.Views;

namespace GameContext.CurrencySystem
{
    public interface ICurrencyViewDatabase
    {
        void RegisterView(ICurrencyView view, CurrencyViewContext context = CurrencyViewContext.None);
        void UnregisterView(ICurrencyView view);
        ICurrencyView GetViewForCurrency(string currencyId, CurrencyViewContext context = CurrencyViewContext.None);
    }
}