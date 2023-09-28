using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.PlatformService;
using Modules.ServiceLocator;
using Services.PlayerDataService;

namespace Services.AdsService
{
    public class AdsService: IAdsService
    {
        private IPlayerDataService _playerDataService;
        private IPlatformService _platformService;
        
        private readonly TimeSpan _adsCooldown = TimeSpan.FromMinutes(5);
        private readonly int _minLevelToShowAds = 5;
        async UniTask IService.Initialize(CancellationToken cancellationToken)
        {
            _playerDataService = await ServiceLocator.GetAsync<IPlayerDataService>(cancellationToken);
            _platformService = await ServiceLocator.GetAsync<IPlatformService>(cancellationToken);
        }

        void IService.Dispose()
        {
        }

        UniTask<bool> IAdsService.ShowRewardedVideo() => TryShowAd(_platformService.ShowRewardedVideo);
        UniTask<bool> IAdsService.ShowInterstitial() => TryShowAd(_platformService.ShowInterstitial);
        UniTask<bool> IAdsService.ShowRewardedInterstitial() => TryShowAd(_platformService.ShowRewardedInterstitial);
  

        private bool CanShow()
        {
            if (_minLevelToShowAds > _playerDataService.Data.Level)
            {
                return false;
            }

            if(DateTime.Now - _playerDataService.Data.AdsLastShownDate < _adsCooldown)
            {
                return false;
            }
            
            return true;
        }
        
        private async UniTask<bool> TryShowAd(Func<UniTask<bool>> action)
        {
            if (!CanShow())
            {
                return false;
            }

            var result = await action.Invoke();
            if (result)
            {
                _playerDataService.Data.AdsLastShownDate = DateTime.Now;
                _playerDataService.Commit();
            }

            return result;
        }
    }
}