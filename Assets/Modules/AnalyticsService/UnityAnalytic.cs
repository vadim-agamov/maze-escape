using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Modules.AnalyticsService
{
    public class UnityAnalytic : IAnalytic
    {
        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>()
        {
#if UNITY_EDITOR
            {"sn", "editor"}
#elif FB
            {"sn", "fb"}
#elif DUMMY_WEBGL
            {"sn", "dummy"}
#endif
        };
        
        async UniTask IAnalytic.Initialize(CancellationToken token)
        {
            Debug.Log($"UnityAnalytic Initialize begin");
            await Unity.Services.Core.UnityServices.InitializeAsync();
            Debug.Log("UnityAnalytic Initialize end");
        }

        void IAnalytic.Start()
        {
            Debug.Log($"UnityAnalytic Start begin");
            Unity.Services.Analytics.AnalyticsService.Instance.StartDataCollection();
            Debug.Log($"UnityAnalytic Start end");
        }

        void IAnalytic.Stop()
        {
            Unity.Services.Analytics.AnalyticsService.Instance.StopDataCollection();
        }

        void IAnalytic.TrackEvent(string eventName, Dictionary<string, object> parameters)
        {
            parameters ??= new Dictionary<string, object>();
            foreach (var (key, value) in _parameters)
            {
                parameters[key] = value;
            }   
            
            Unity.Services.Analytics.AnalyticsService.Instance.CustomData(eventName, parameters);
        }
    }
}