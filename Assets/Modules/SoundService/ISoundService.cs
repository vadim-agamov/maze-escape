using Modules.ServiceLocator;

namespace Modules.SoundService
{
    public interface ISoundService: IService
    {
        void Play(string soundId, bool loop = false);
        void Stop(string soundId);
        void Mute();
        void UnMute();
        bool IsMuted { get;}
    }
}