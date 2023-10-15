using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using Modules.SoundService;
using Services.AdsService;

namespace Actions
{
    public class ShowInterstitialAction
    {
        private IAdsService AdsService { get; } = ServiceLocator.Get<IAdsService>();
        private ISoundService SoundService { get; } = ServiceLocator.Get<ISoundService>();

        public async UniTask Execute()
        {
            var isMuted = SoundService.IsMuted;
            if (!isMuted)
            {
                SoundService.Mute();
            }

            await AdsService.ShowInterstitial();

            if (!isMuted)
            {
                SoundService.UnMute();
            }
        }
    }
}