namespace ProjectContext
{
    public interface ILoadingScreenHandle
    {
        void Show();
        void Hide();
        void SetProgress(float progress, string msg);
    }
}