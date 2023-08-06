using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;

namespace Modules.SoundService
{
    public interface ISoundService: IService
    {
        void PlayLoop(string soundId);
        UniTask Play(string soundId);
        UniTask Stop(string soundId);
        void Mute();
        void UnMute();
        bool IsMuted { get;}
    }
}