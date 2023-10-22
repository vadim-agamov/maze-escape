using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;

namespace Services.AdsService
{
    public interface IAdsService: IService
    {
        UniTask<bool> ShowRewardedVideo(CancellationToken token);
        UniTask<bool> ShowInterstitial(CancellationToken token);
        UniTask<bool> ShowRewardedInterstitial(CancellationToken token);
    }
}