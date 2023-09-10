using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using Modules.SocialNetworkService;
using Modules.SoundService;

namespace Actions
{
    public class ShowInterstitialAction
    {
        private readonly ISocialNetworkService _snService = ServiceLocator.Get<ISocialNetworkService>();
        private readonly ISoundService _soundService = ServiceLocator.Get<ISoundService>();

        public async UniTask Execute()
        {
            _soundService.Mute();
            await _snService.ShowInterstitial();
            _soundService.UnMute();
        }
    }
}