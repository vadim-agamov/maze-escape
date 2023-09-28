using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;

namespace Services.AdsService
{
    public interface IAdsService: IService
    {
        UniTask<bool> ShowRewardedVideo();
        UniTask<bool> ShowInterstitial();
        UniTask<bool> ShowRewardedInterstitial();
    }
}