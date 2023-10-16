#if YANDEX
using Cysharp.Threading.Tasks;

namespace Modules.PlatformService.YandexPlatformService
{
    public partial class YandexPlatformService
    {
        void IPlatformService.PreloadRewardedInterstitial()
        {
        }

        UniTask<bool> IPlatformService.ShowRewardedInterstitial() => UniTask.FromResult<bool>(false);
    }
}
#endif