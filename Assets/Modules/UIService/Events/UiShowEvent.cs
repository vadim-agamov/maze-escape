namespace Modules.UIService.Events
{
    public struct UiShowEvent
    {
        public UIModel Model { get; }

        public UiShowEvent(UIModel model)
        {
            Model = model;
        }
    }
}