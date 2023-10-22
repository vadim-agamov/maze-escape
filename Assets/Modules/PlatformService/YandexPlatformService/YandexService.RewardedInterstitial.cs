#if YANDEX
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Modules.PlatformService.YandexPlatformService
{
    public partial class YandexPlatformService
    {
        void IPlatformService.PreloadRewardedInterstitial()
        {
        }

        UniTask<bool> IPlatformService.ShowRewardedInterstitial(CancellationToken token) => UniTask.FromResult<bool>(false);
    }
}
#endif