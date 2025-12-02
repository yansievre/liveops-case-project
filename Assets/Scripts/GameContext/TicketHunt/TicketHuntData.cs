using System;
using System.Collections.Generic;
using GameContext.CurrencySystem;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace GameContext.TicketHunt
{
    [Serializable]
    public class TicketHuntData
    {
        [SerializeField]
        private int _uniqueEventId;
        [SerializeField]
        private string _currencyId;
        [SerializeField]
        private SerializableDateTime _startTime;
        [SerializeField]
        private SerializableDateTime _endTime;
        [SerializeField]
        private List<TicketHuntStep> _steps;
        
        public IReadOnlyList<TicketHuntStep> Steps => _steps;

        public DateTime UTCStartTime => _startTime.DateTime.ToUniversalTime();
        public DateTime UTCEndTime => _endTime.DateTime.ToUniversalTime();

        public int UniqueEventId => _uniqueEventId;
        public string CurrencyId => _currencyId;
    }

    [Serializable]
    public class TicketHuntStep
    {
        [SerializeField]
        private int _requiredCurrency;
        [SerializeField]
        private TransactionPackage _rewardPackage;
        
        
        public int RequiredCurrency => _requiredCurrency;
        public TransactionPackage RewardPackage => _rewardPackage;
    }
}