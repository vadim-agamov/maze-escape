using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using Modules.SoundService;
using Services.AdsService;

namespace Actions
{
    public class ShowInterstitialAction
    {
        private readonly IAdsService _adsService = ServiceLocator.Get<IAdsService>();
        private readonly ISoundService _soundService = ServiceLocator.Get<ISoundService>();

        public async UniTask Execute()
        {
            var isMuted = _soundService.IsMuted;
            if (!isMuted)
            {
                _soundService.Mute();
            }

            await _adsService.ShowInterstitial();

            if (!isMuted)
            {
                _soundService.UnMute();
            }
        }
    }
}