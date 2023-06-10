namespace Modules.UIService.Events
{
    public class UiShowEvent
    {
        public readonly UIModel Model;

        public UiShowEvent(UIModel model)
        {
            Model = model;
        }
    }
}