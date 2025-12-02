using GlobalContext.Persistence;
using UnityEngine;

namespace ProjectContext
{
    public class SaveOnExit
    {
        private readonly IGameStateHandler _gameStateHandler;

        public SaveOnExit(IGameStateHandler gameStateHandler)
        {
            _gameStateHandler = gameStateHandler;
            Application.quitting += Save;
        }
        
        private void Save()
        {
            _gameStateHandler.SaveGameState();
        }
    }
}