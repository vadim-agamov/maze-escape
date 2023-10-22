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
        private GamePlayerDataService.GamePlayerDataService _playerDataService;
        private IPlatformService _platformService;
        private IAnalyticsService _analyticsService;
        
        private readonly TimeSpan _adsCooldown = TimeSpan.FromMinutes(5);
        private readonly int _minLevelToShowAds = 5;
        async UniTask IService.Initialize(CancellationToken cancellationToken)
        {
            _playerDataService = await ServiceLocator.GetAsync<GamePlayerDataService.GamePlayerDataService>(cancellationToken);
            _platformService = await ServiceLocator.GetAsync<IPlatformService>(cancellationToken);
            _analyticsService = await ServiceLocator.GetAsync<IAnalyticsService>(cancellationToken);
        }

        void IService.Dispose()
        {
        }

        UniTask<bool> IAdsService.ShowRewardedVideo(CancellationToken token) => TryShowAd(_platformService.ShowRewardedVideo, token);
        UniTask<bool> IAdsService.ShowInterstitial(CancellationToken token) => TryShowAd(_platformService.ShowInterstitial, token);
        UniTask<bool> IAdsService.ShowRewardedInterstitial(CancellationToken token) => TryShowAd(_platformService.ShowRewardedInterstitial, token);
  

        private bool CanShow()
        {
            if (_minLevelToShowAds > _playerDataService.PlayerData.Level)
            {
                return false;
            }

            if(DateTime.Now - _playerDataService.PlayerData.AdsLastShownDate < _adsCooldown)
            {
                return false;
            }
            
            return true;
        }
        
        private async UniTask<bool> TryShowAd(Func<CancellationToken, UniTask<bool>> action, CancellationToken token)
        {
            if (!CanShow())
            {
                _analyticsService.TrackEvent("AdsNotShown");
                return false;
            }

            var result = await action.Invoke(token);
            _analyticsService.TrackEvent("AdsShown", new Dictionary<string, object>{{"success", result}});

            if (result)
            {
                _playerDataService.PlayerData.AdsLastShownDate = DateTime.Now;
            }
            return result;
        }
    }
}