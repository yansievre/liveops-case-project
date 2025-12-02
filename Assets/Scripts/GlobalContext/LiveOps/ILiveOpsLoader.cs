using Cysharp.Threading.Tasks;
using R3;

namespace GameContext.TicketHunt.DataLoader
{
    public interface ILiveOpsLoader
    {
        ReadOnlyReactiveProperty<TicketHuntData> TicketHuntData { get; }
        UniTask LoadLiveOpsData();
    }
}