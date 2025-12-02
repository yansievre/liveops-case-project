using System;
using GameContext.CurrencySystem;
using UnityEngine;

namespace GameContext.VisualTransactions
{
    public interface IVisualTransactionManager
    {
        VisualTransactionHandle PerformVisualOnlyTransaction(Transaction transaction, RectTransform source, CurrencyViewContext targetContext = CurrencyViewContext.None);
        VisualTransactionHandle CreateVisualOnlyTransaction(Transaction transaction, CurrencyViewContext targetContext = CurrencyViewContext.None);
    }
}