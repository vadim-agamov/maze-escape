using Modules.ServiceLocator;

namespace Modules.CheatService
{
    public interface ICheatService : IService
    {
        void Show();
        void Hide();
        void RegisterCheatProvider(ICheatsProvider cheatsProvider);
        void UnRegisterCheatProvider(ICheatsProvider cheatsProvider);
    }
}