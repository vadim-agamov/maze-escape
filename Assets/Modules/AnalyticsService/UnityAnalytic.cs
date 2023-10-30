using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using static Unity.Services.Analytics.AnalyticsService;
using static Unity.Services.Core.UnityServices;

namespace Modules.AnalyticsService
{
    public class UnityAnalytic : IAnalytic
    {
        async UniTask IAnalytic.Initialize(CancellationToken token)
        {
            await InitializeAsync();
        }

        void IAnalytic.Start() => Instance.StartDataCollection();

        void IAnalytic.Stop() => Instance.StopDataCollection();

        void IAnalytic.TrackEvent(string eventName, Dictionary<string, object> parameters) => Instance.CustomData(eventName, parameters);
    }
}