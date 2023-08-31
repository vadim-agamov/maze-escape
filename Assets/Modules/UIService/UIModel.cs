namespace Modules.UIService
{
    public abstract class UIModel
    {
        public UIViewBase View { get; private set; }

        public void AttachView(UIViewBase view)
        {
            View = view;
            View.SetModel(this);
        }
        
        public void DeattachView()
        {
            View.UnsetModel();
            View = null;
        }
        
        public void UpdateModel()
        {
            View.OnUpdateModel();
        }
    }
}