using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Modules.AnalyticsService
{
    public class SnAnalytic : IAnalytic
    {
        UniTask IAnalytic.Initialize(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        void IAnalytic.Start()
        {
        }

        void IAnalytic.Stop()
        {
        }

        void IAnalytic.TrackEvent(string eventName, Dictionary<string, object> parameters)
        {
            SnBridge.SnBridge.Instance.LogEvent(eventName, parameters);
        }
    }
}