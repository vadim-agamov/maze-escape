using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Modules.SocialNetworkService.FbSocialNetworkService
{
    public partial class FbSocialNetworkService
    {
        [DllImport("__Internal")]
        private static extern void FbShowInterstitial();
        
        [DllImport("__Internal")]
        private static extern void FbPreloadInterstitial(string id);
        
        private const string InterstitialAdlId = "644999433795437_683351359960244";

        private readonly AdController _interstitialAdController = new AdController(InterstitialAdlId, FbPreloadInterstitial, FbShowInterstitial);

        void ISocialNetworkService.PreloadInterstitial() => _interstitialAdController.Preload();
        UniTask<bool> ISocialNetworkService.ShowInterstitial() => _interstitialAdController.Show();

        [UsedImplicitly]
        public void OnInterstitialLoaded() => _interstitialAdController.OnLoaded();

        [UsedImplicitly]
        public void OnInterstitialNotLoaded() => _interstitialAdController.OnNotLoaded();

        [UsedImplicitly]
        public void OnInterstitialShown() => _interstitialAdController.OnShown();

        [UsedImplicitly]
        public void OnInterstitialNotShown(string e) => _interstitialAdController.OnNotShown(e);
    }
}