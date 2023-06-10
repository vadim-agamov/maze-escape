using Cysharp.Threading.Tasks;
using Modules.UIService;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelWinModel: UIModel
    {
        public void OnClosed()
        {
            this.HideAndClose(Bootstrapper.SessionToken).Forget();
        }
    }
    
    public class LevelWinView: UIView<LevelWinModel>
    {
        [SerializeField] 
        private Button _closeHandler;
        
        protected override void OnSetModel()
        {
            _closeHandler.onClick.AddListener(Model.OnClosed);
        }

        protected override void OnUnsetModel()
        {
            _closeHandler.onClick.RemoveAllListeners();
        }
    }
}