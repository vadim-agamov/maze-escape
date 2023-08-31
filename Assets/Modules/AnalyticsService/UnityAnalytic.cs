using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Modules.AnalyticsService
{
    public class UnityAnalytic : IAnalytic
    {
        // private UniTaskCompletionSource _initializationTokenSource;

        async UniTask IAnalytic.Initialize(CancellationToken token)
        {
            Debug.Log($"UnityAnalytic Initialize begin");
            // _initializationTokenSource = new UniTaskCompletionSource();
            await Unity.Services.Core.UnityServices.InitializeAsync();
                // .ContinueWith(_ => _initializationTokenSource.TrySetResult(), token);
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
            Unity.Services.Analytics.AnalyticsService.Instance.CustomData(eventName, parameters);
        }
    }
}