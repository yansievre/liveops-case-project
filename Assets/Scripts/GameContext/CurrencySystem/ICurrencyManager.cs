namespace GameContext.CurrencySystem
{
    public interface ICurrencyManager
    {
        CurrencyHandle GetCurrencyHandle(string currencyId);
        string GetCurrencyIdFromVisibleName(string visibleName);
    }
}