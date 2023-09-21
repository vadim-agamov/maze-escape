using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using UnityEngine;

namespace Modules.AnalyticsService
{
    public class AnalyticsService: IAnalyticsService
    {
        private readonly List<IAnalytic> _analytics = new List<IAnalytic>();

        public AnalyticsService()
        {
            _analytics.Add(new UnityAnalytic());
            _analytics.Add(new SnAnalytic());
        }

        async UniTask IService.Initialize(CancellationToken cancellationToken)
        {
            Debug.Log($"[{nameof(AnalyticsService)}] Initialize begin");
            await UniTask.WhenAll(_analytics.Select(a => a.Initialize(cancellationToken)));
            Debug.Log($"[{nameof(AnalyticsService)}] Initialize end");
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