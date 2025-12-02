using Cysharp.Threading.Tasks;

namespace GlobalContext.Persistence
{
    public interface IGameStateHandler
    {
        public GameState CurrentState { get; }
        UniTask LoadGameState();

        UniTask SaveGameStateAsync();

        void SaveGameState();
    }
}