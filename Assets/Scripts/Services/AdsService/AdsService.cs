using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.AnalyticsService;
using Modules.PlatformService;
using Modules.ServiceLocator;

namespace Services.AdsService
{
    public class AdsService: IAdsService
    {
        private GamePlayerDataService.GamePlayerDataService PlayerDataService { get; set; }
        private IAnalyticsService AnalyticsService { get; set; } 
        private IPlatformService PlatformService { get; set; }
        
        private readonly TimeSpan _adsCooldown = TimeSpan.FromMinutes(5);
        private readonly int _minLevelToShowAds = 5;
        UniTask IService.Initialize(CancellationToken cancellationToken)
        {
            PlatformService =  ServiceLocator.Get<IPlatformService>();
            PlayerDataService = ServiceLocator.Get<GamePlayerDataService.GamePlayerDataService>();
            AnalyticsService = ServiceLocator.Get<IAnalyticsService>();
            
            return UniTask.CompletedTask;
        }
        
        void IService.Dispose()
        {
        }

        UniTask<bool> IAdsService.ShowRewardedVideo(CancellationToken token) => TryShowAd(PlatformService.ShowRewardedVideo, token);
        UniTask<bool> IAdsService.ShowInterstitial(CancellationToken token) => TryShowAd(PlatformService.ShowInterstitial, token);
        UniTask<bool> IAdsService.ShowRewardedInterstitial(CancellationToken token) => TryShowAd(PlatformService.ShowRewardedInterstitial, token);
  

        private bool CanShow()
        {
            if (_minLevelToShowAds > PlayerDataService.PlayerData.Level)
            {
                return false;
            }

            if(DateTime.Now - PlayerDataService.PlayerData.AdsLastShownDate < _adsCooldown)
            {
                return false;
            }
            
            return true;
        }
        
        private async UniTask<bool> TryShowAd(Func<CancellationToken, UniTask<bool>> action, CancellationToken token)
        {
            if (!CanShow())
            {
                AnalyticsService.TrackEvent("AdsNotShown");
                return false;
            }

            var result = await action.Invoke(token);
            AnalyticsService.TrackEvent("AdsShown");

            if (result)
            {
                PlayerDataService.PlayerData.AdsLastShownDate = DateTime.Now;
            }
            return result;
        }
    }
}