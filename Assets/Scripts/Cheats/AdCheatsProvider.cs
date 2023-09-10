using Cysharp.Threading.Tasks;
using Modules.CheatService;
using Modules.CheatService.Controls;
using Modules.ServiceLocator;
using Modules.SocialNetworkService;

namespace Cheats
{
    public class AdCheatsProvider : ICheatsProvider
    {
        private readonly CheatButton _showRewardedInterstitial;
        private readonly CheatButton _showInterstitial;
        private readonly CheatButton _showRewardedVideo;
        
        
        private readonly CheatLabel _isRewardedAdShownLabel;
        private readonly CheatLabel _isInterstitialAdShownLabel;
        private readonly CheatLabel _isRewardedVideoAdShownLabel;
        
        private bool _isRewardedAdShown;
        private bool _isInterstitialAdShown;
        private bool _isRewardedVideoAdShown;

        public AdCheatsProvider(ICheatService cheatService)
        {
            _showRewardedInterstitial = new CheatButton(cheatService, "Show Rewarded Interstitial", () =>
            {
                ServiceLocator.Get<ISocialNetworkService>()
                    .ShowRewardedInterstitial()
                    .ContinueWith(x => _isRewardedAdShown = x);
            });
            _isRewardedAdShownLabel = new CheatLabel(()=> $"Is Rewarded Interstitial Shown: {_isRewardedAdShown}");
            
            _showInterstitial = new CheatButton(cheatService, "Show Interstitial", () =>
            {
                ServiceLocator.Get<ISocialNetworkService>()
                    .ShowInterstitial()
                    .ContinueWith(x => _isInterstitialAdShown = x);
            });
            _isInterstitialAdShownLabel = new CheatLabel(()=> $"Is Interstitial Shown: {_isInterstitialAdShown}");
            
            _showRewardedVideo = new CheatButton(cheatService, "Show Rewarded Video", () =>
            {
                ServiceLocator.Get<ISocialNetworkService>()
                    .ShowRewardedVideo()
                    .ContinueWith(x => _isRewardedVideoAdShown = x);
            });
            _isRewardedVideoAdShownLabel = new CheatLabel(()=> $"Is Rewarded Video Shown: {_isRewardedVideoAdShown}");
        }

        public void OnGUI()
        {
            _showInterstitial.OnGUI();
            _isInterstitialAdShownLabel.OnGUI();
            
            _showRewardedInterstitial.OnGUI();
            _isRewardedAdShownLabel.OnGUI();
            
            _showRewardedVideo.OnGUI();
            _isRewardedVideoAdShownLabel.OnGUI();
        }

        public string Id => "Advertising";
    }
}