using Cysharp.Threading.Tasks;
using GameContext.TicketHunt.DataLoader;
using GlobalContext.Persistence;
using ProjectContext;
using UnityEngine.SceneManagement;
using Zenject;

namespace GlobalContext
{
    public class GameLoader : IInitializable
    {
        private readonly ILoadingScreenHandle _loadingScreenHandle;
        private readonly IGameStateHandler _gameStateHandler;
        private readonly ILiveOpsLoader _liveOpsLoader;
        private readonly ZenjectSceneLoader _zenjectSceneLoader;

        public GameLoader(
            ILoadingScreenHandle loadingScreenHandle,
            IGameStateHandler gameStateHandler,
            ILiveOpsLoader liveOpsLoader,
            ZenjectSceneLoader zenjectSceneLoader)
        {
            _loadingScreenHandle = loadingScreenHandle;
            _gameStateHandler = gameStateHandler;
            _liveOpsLoader = liveOpsLoader;
            _zenjectSceneLoader = zenjectSceneLoader;
        }

        public void Initialize()
        {
            UniTask.NextFrame().ContinueWith(() => InitializeAsync().Forget()).Forget();
        }

        private async UniTask InitializeAsync()
        {
            _loadingScreenHandle.Show();
            
            {
                _loadingScreenHandle.SetProgress(0.25f, "Loading data");
                await _gameStateHandler.LoadGameState();
            }
            
            {
                _loadingScreenHandle.SetProgress(0.5f, "Loading event data");
                await _liveOpsLoader.LoadLiveOpsData();
            }
            
            {
                _loadingScreenHandle.SetProgress(0.75f, "Finalizing");
                var loadSceneAsyncOperation = _zenjectSceneLoader.LoadSceneAsync(1, LoadSceneMode.Additive,
                    container => { container.BindInstance(_gameStateHandler.CurrentState).AsSingle(); });

                await loadSceneAsyncOperation.ToUniTask();
            }
            
            _loadingScreenHandle.Hide();
        }
    }
}