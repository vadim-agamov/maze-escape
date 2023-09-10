using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Modules.SocialNetworkService.FbSocialNetworkService
{
    public partial class FbSocialNetworkService
    {
        [DllImport("__Internal")]
        private static extern void FbShowRewardedInterstitial();
        
        [DllImport("__Internal")]
        private static extern void FbPreloadRewardedInterstitial(string id);
        
        private const string RewardedInterstitialAdlId = "644999433795437_683351186626928";

        private readonly AdController _rewardedInterstitialAdController = new AdController(RewardedInterstitialAdlId, FbPreloadRewardedInterstitial, FbShowRewardedInterstitial);

        void ISocialNetworkService.PreloadRewardedInterstitial() => _rewardedInterstitialAdController.Preload();
        
        UniTask<bool> ISocialNetworkService.ShowRewardedInterstitial() => _rewardedInterstitialAdController.Show();

        [UsedImplicitly]
        public void OnRewardedInterstitialLoaded() => _rewardedInterstitialAdController.OnLoaded();

        [UsedImplicitly]
        public void OnRewardedInterstitialNotLoaded() => _rewardedInterstitialAdController.OnNotLoaded();

        [UsedImplicitly]
        public void OnRewardedInterstitialShown() => _rewardedInterstitialAdController.OnShown();

        [UsedImplicitly]
        public void OnRewardedInterstitialNotShown(string e) => _rewardedInterstitialAdController.OnNotShown(e);
    }
}