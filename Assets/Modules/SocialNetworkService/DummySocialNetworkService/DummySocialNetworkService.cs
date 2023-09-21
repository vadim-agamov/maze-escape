using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using UnityEngine;

namespace Modules.SocialNetworkService.DummySocialNetworkService
{
    public class DummySocialNetworkService : MonoBehaviour, ISocialNetworkService
    {
        UniTask IService.Initialize(CancellationToken cancellationToken)
        {
            DontDestroyOnLoad(gameObject);
            return UniTask.CompletedTask;
        }

        void IService.Dispose()
        {
        }

        string ISocialNetworkService.GetUserId() => SystemInfo.deviceUniqueIdentifier;

        UniTask<string> ISocialNetworkService.LoadPlayerProgress() => UniTask.FromResult(string.Empty);

        UniTask ISocialNetworkService.SavePlayerProgress(string playerProgress) => UniTask.CompletedTask;

        void ISocialNetworkService.PreloadRewardedVideo()
        {
        }

        async UniTask<bool> ISocialNetworkService.ShowRewardedVideo()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            return true;
        }

        void ISocialNetworkService.PreloadInterstitial()
        {
        }

        async UniTask<bool> ISocialNetworkService.ShowInterstitial()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            return true;
        }

        void ISocialNetworkService.PreloadRewardedInterstitial()
        {
        }

        async UniTask<bool> ISocialNetworkService.ShowRewardedInterstitial()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            return true;
        }

        void ISocialNetworkService.LogEvent(string eventName, Dictionary<string, object> parameters)
        {
        }
    }
}