namespace Modules.UIService.Events
{
    public struct UiHideEvent
    {
        public UIModel Model { get; }

        public UiHideEvent(UIModel model)
        {
            Model = model;
        }
    }
}