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
        private static extern void YandexShowRewardedVideo();

        private readonly AdController _rewardedVideoAdController = new AdController(YandexShowRewardedVideo);

        void IPlatformService.PreloadRewardedVideo() { }

        UniTask<bool> IPlatformService.ShowRewardedVideo(CancellationToken token) => _rewardedVideoAdController.Show();
        
        [UsedImplicitly]
        public void YandexOnRewardedVideoShown() => _rewardedVideoAdController.OnShown();

        [UsedImplicitly]
        public void YandexOnRewardedVideoNotShown(string e) => _rewardedVideoAdController.OnNotShown(e);
    }
}
#endif