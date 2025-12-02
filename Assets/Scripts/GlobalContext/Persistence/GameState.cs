using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace GlobalContext.Persistence
{
    [Serializable]
    public class GameState
    {
        [SerializeField]
        private SerializableDictionary<string, long> _currencyData = new SerializableDictionary<string, long>();
        [SerializeField]
        private LiveOpsStates _liveOpsStates = new LiveOpsStates();

        public IDictionary<string, long> CurrencyData => _currencyData;
        public LiveOpsStates LiveOpsStates => _liveOpsStates;
    }

    [Serializable]
    public class LiveOpsStates
    {        
        [SerializeField]
        private SerializableDictionary<int, TicketHuntState> _ticketHuntData = new SerializableDictionary<int, TicketHuntState>();

        public IDictionary<int, TicketHuntState> TicketHuntData => _ticketHuntData;
    }

    [Serializable]
    public class TicketHuntState
    {
        public int currentLevel = 0;
    }
}