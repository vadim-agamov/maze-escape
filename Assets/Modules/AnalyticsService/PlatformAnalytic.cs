using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.PlatformService;

namespace Modules.AnalyticsService
{
    public class PlatformAnalytic : IAnalytic
    {
        private IPlatformService _platformService;

        async UniTask IAnalytic.Initialize(CancellationToken token)
        {
            _platformService = await ServiceLocator.ServiceLocator.GetAsync<IPlatformService>(token);
        }

        void IAnalytic.Start()
        {
        }

        void IAnalytic.Stop()
        {
        }

        void IAnalytic.TrackEvent(string eventName, Dictionary<string, object> parameters)
        {
            _platformService.LogEvent(eventName, parameters);
        }
    }
}