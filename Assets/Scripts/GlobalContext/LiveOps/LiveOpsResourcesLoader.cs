using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace GameContext.TicketHunt.DataLoader
{
    public class LiveOpsResourcesLoader : IDisposable, ILiveOpsLoader
    {
        private readonly ReactiveProperty<TicketHuntData> _ticketHuntData;

        public LiveOpsResourcesLoader()
        {
            _ticketHuntData = new ReactiveProperty<TicketHuntData>(null);
        }
        
        public ReadOnlyReactiveProperty<TicketHuntData> TicketHuntData => _ticketHuntData;
        
        public async UniTask LoadLiveOpsData()
        {
            var loadResult = await Resources.LoadAsync<TextAsset>("TicketHuntData").ToUniTask();
            var textAsset = (TextAsset) loadResult;

            try
            {
                var ticketHuntData = JsonUtility.FromJson<TicketHuntData>(textAsset.text);
                _ticketHuntData.Value = ticketHuntData;
            }catch (Exception e)
            {
                _ticketHuntData.Value = null;
            }
        }

        public void Dispose()
        {
            _ticketHuntData?.Dispose();
        }
    }
}