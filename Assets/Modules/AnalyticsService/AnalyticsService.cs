using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;

namespace Modules.AnalyticsService
{
    public class AnalyticsService: IAnalyticsService
    {
        private readonly List<IAnalytic> _analytics = new List<IAnalytic>();

        public AnalyticsService()
        {
            _analytics.Add(new UnityAnalytic());
        }

        async UniTask IService.Initialize(IProgress<float> progress, CancellationToken cancellationToken)
        {
            await UniTask.WhenAll(_analytics.Select(a => a.Initialize(cancellationToken)));
        }

        void IService.Dispose()
        {
        }
        
        void IAnalyticsService.Start()
        {
            _analytics.ForEach(a => a.Start());
        }

        void IAnalyticsService.Stop()
        {
            _analytics.ForEach(a => a.Stop());
        }

        void IAnalyticsService.TrackEvent(string eventName, Dictionary<string, object> parameters)
        {
            _analytics.ForEach(a => a.TrackEvent(eventName, parameters));
        }
    }
}