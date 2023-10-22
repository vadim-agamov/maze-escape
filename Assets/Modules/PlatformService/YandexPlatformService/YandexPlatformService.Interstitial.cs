#if YANDEX
using System.Runtime.InteropServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Modules.PlatformService.YandexPlatformService
{
    public partial class YandexPlatformService
    {
        [DllImport("__Internal")]
        private static extern void YandexShowInterstitial();
        
        private readonly AdController _interstitialAdController = new AdController(YandexShowInterstitial);

        void IPlatformService.PreloadInterstitial() {}

        UniTask<bool> IPlatformService.ShowInterstitial(CancellationToken token) => _interstitialAdController.Show();
        

        [UsedImplicitly]
        public void YandexOnInterstitialShown() => _interstitialAdController.OnShown();
        
        [UsedImplicitly]
        public void YandexOnInterstitialNotShown(string e) => _interstitialAdController.OnNotShown(e);
    }
}
#endif