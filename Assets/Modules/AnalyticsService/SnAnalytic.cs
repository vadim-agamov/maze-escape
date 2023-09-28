using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.PlatformService;

namespace Modules.AnalyticsService
{
    public class SnAnalytic : IAnalytic
    {
        private IPlatformService _snService;

        async UniTask IAnalytic.Initialize(CancellationToken token)
        {
            _snService = await ServiceLocator.ServiceLocator.GetAsync<IPlatformService>(token);
        }

        void IAnalytic.Start()
        {
        }

        void IAnalytic.Stop()
        {
        }

        void IAnalytic.TrackEvent(string eventName, Dictionary<string, object> parameters)
        {
            _snService.LogEvent(eventName, parameters);
        }
    }
}