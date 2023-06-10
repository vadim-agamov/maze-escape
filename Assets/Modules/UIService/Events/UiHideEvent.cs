namespace Modules.UIService.Events
{
    public class UiHideEvent
    {
        public readonly UIModel Model;

        public UiHideEvent(UIModel model)
        {
            Model = model;
        }
    }
}