using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;


namespace Modules.AnalyticsService
{
    public class UnityAnalytic : IAnalytic
    {
        private UniTaskCompletionSource _initializationTokenSource;

        UniTask IAnalytic.Initialize(CancellationToken token)
        {
            _initializationTokenSource = new UniTaskCompletionSource();
            Unity.Services.Core.UnityServices.InitializeAsync()
                .ContinueWith(_ => _initializationTokenSource.TrySetResult(), token);
            return _initializationTokenSource.Task.AttachExternalCancellation(token);
        }

        void IAnalytic.Start()
        {
            Unity.Services.Analytics.AnalyticsService.Instance.StartDataCollection();
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