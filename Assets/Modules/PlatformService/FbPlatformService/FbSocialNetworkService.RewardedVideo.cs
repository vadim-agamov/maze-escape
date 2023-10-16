#if FB

using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace Modules.PlatformService.FbPlatformService
{
    public partial class FbPlatformService
    {
        [DllImport("__Internal")]
        private static extern void FbShowRewardedVideo();
        
        [DllImport("__Internal")]
        private static extern void FbPreloadRewardedVideo(string id);
        
        private const string RewardedVideoAdlId = "644999433795437_685189986443048";

        private readonly AdController _rewardedVideoAdController = new AdController(RewardedVideoAdlId, FbPreloadRewardedVideo, FbShowRewardedVideo);

        void IPlatformService.PreloadRewardedVideo() => _rewardedVideoAdController.Preload();
        UniTask<bool> IPlatformService.ShowRewardedVideo() => _rewardedVideoAdController.Show();

        [UsedImplicitly]
        public void OnRewardedVideoLoaded() => _rewardedVideoAdController.OnLoaded();

        [UsedImplicitly]
        public void OnRewardedVideoNotLoaded() => _rewardedVideoAdController.OnNotLoaded();

        [UsedImplicitly]
        public void OnRewardedVideoShown() => _rewardedVideoAdController.OnShown();

        [UsedImplicitly]
        public void OnRewardedVideoNotShown(string e) => _rewardedVideoAdController.OnNotShown(e);
    }
}
#endif