using Modules.ServiceLocator;

namespace Modules.UIService
{
    public interface INavigationService : IService
    {
        void OnShown(UIModel model);
        void OnHidden(UIModel model);
        void SetInteractiveState(bool state);
    }
}