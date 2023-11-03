using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;

namespace Modules.AnalyticsService
{
    public class AnalyticsService: IAnalyticsService
    {
        private readonly List<IAnalytic> _analytics = new List<IAnalytic>();
        
        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>()
        {
#if UNITY_EDITOR
            {"sn", "editor"}
#elif FB
            {"sn", "fb"}
#elif YANDEX
            {"sn", "yandex"}
#elif CRAZY 
            {"sn", "crazy"}
#elif DUMMY_WEBGL
            {"sn", "dummy"}
#endif
        };
        
        public AnalyticsService()
        {
#if !UNITY_EDITOR
            _analytics.Add(new UnityAnalytic());
            _analytics.Add(new PlatformAnalytic());
#endif
        }

        async UniTask IService.Initialize(CancellationToken cancellationToken)
        {
            await UniTask.WhenAll(_analytics.Select(a => a.Initialize(cancellationToken)));
        }
        
        void IService.Dispose()
        {
        }
        
        void IAnalyticsService.Start() => _analytics.ForEach(a => a.Start());
        void IAnalyticsService.Stop() => _analytics.ForEach(a => a.Stop());

        void IAnalyticsService.TrackEvent(string eventName, Dictionary<string, object> parameters)
        {
            parameters ??= new Dictionary<string, object>();
            foreach (var (key, value) in _parameters)
            {
                parameters[key] = value;
            }  
            
            _analytics.ForEach(a => a.TrackEvent(eventName, parameters));
        }
    }
}