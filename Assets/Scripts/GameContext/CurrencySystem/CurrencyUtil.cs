using System;

namespace GameContext.CurrencySystem
{
    [Serializable]
    public struct Transaction
    {
        public string id;
        public long amount;
    }

    [Serializable]
    public class TransactionPackage
    {
        public Transaction[] transactions;
    }
}