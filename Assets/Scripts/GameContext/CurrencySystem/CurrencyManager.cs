using System;
using System.Collections.Generic;
using GlobalContext.Persistence;
using R3;
using Zenject;

namespace GameContext.CurrencySystem
{
    public class CurrencyManager : IDisposable, ICurrencyManager
    {
        private readonly CurrencyDefinitions _currencyDefinitions;
        private readonly IGameStateHandler _gameStateHandler;
        private Dictionary<string, CurrencyHandle> _currencyHandles = new Dictionary<string, CurrencyHandle>();

        public CurrencyManager(CurrencyDefinitions currencyDefinitions, IGameStateHandler gameStateHandler)
        {
            _currencyDefinitions = currencyDefinitions;
            _gameStateHandler = gameStateHandler;
            
            
            bool addedNewEntryToState = false;
            foreach (var currencyDefinition in _currencyDefinitions.CurrencyDefinitionList)
            {
                if (!CurrentState.CurrencyData.ContainsKey(currencyDefinition.UniqueIdentifier))
                {
                    CurrentState.CurrencyData[currencyDefinition.UniqueIdentifier] = 0;
                    addedNewEntryToState = true;
                }
            }
            // Optionally, remove currencies that are no longer defined, but long term events may rely on them
            
            if(addedNewEntryToState)
                _gameStateHandler.SaveGameState();
        }
        
        private GameState CurrentState => _gameStateHandler.CurrentState;

        public CurrencyHandle GetCurrencyHandle(string currencyId)
        {
            if (_currencyHandles.TryGetValue(currencyId, out var handle))
                return handle;
            
            var currencyDefinition = _currencyDefinitions.CurrencyDefinitionList.Find(x => x.UniqueIdentifier == currencyId);
            if (currencyDefinition == null)
                throw new KeyNotFoundException($"Currency with ID {currencyId} not found");

            var currencyHandle = new CurrencyHandle(currencyDefinition, _gameStateHandler);
            _currencyHandles[currencyId] = currencyHandle;
            return currencyHandle;
        }

        public string GetCurrencyIdFromVisibleName(string visibleName)
        {
            var currencyDefinition = _currencyDefinitions.CurrencyDefinitionList.Find(x => x.VisibleName == visibleName);
            if (currencyDefinition == null)
                throw new KeyNotFoundException($"Currency with visible name {visibleName} not found");
            return currencyDefinition.UniqueIdentifier;
        }

        public void Dispose()
        {
            foreach (var handle in _currencyHandles.Values)
            {
                handle.Dispose();
            }
        }
    }
}