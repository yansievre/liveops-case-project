using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GlobalContext.Persistence
{
    public class PlayerPrefsGameStateHandler : IGameStateHandler
    {
        private const string K_GameDataKey = "gameData";
        public GameState CurrentState { get; private set; }
        public UniTask LoadGameState()
        {
            var dataJson = PlayerPrefs.GetString(K_GameDataKey, string.Empty);
            
            if(!string.IsNullOrEmpty(dataJson))
            {
                CurrentState = JsonUtility.FromJson<GameState>(dataJson);

                return UniTask.CompletedTask;
            }
            
            CurrentState = new GameState();
            return UniTask.CompletedTask;
        }

        public UniTask SaveGameStateAsync()
        {
            SaveGameState();
            return UniTask.CompletedTask;
        }

        public void SaveGameState()
        {
            var dataJson = JsonUtility.ToJson(CurrentState);
            PlayerPrefs.SetString(K_GameDataKey, dataJson);
            PlayerPrefs.Save();
        }

    }
}