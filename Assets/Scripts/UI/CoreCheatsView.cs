using Actions;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using Modules.UIService;
using Services;
using States;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CoreCheatsModel : UIModel
    {
        public async UniTask Loose()
        {
            await new LooseLevelAction().Execute(Bootstrapper.SessionToken);
            await this.HideAndClose(Bootstrapper.SessionToken);
        }

        public async UniTask ResetProgress()
        {
            var playerDataService = ServiceLocator.Get<IPlayerDataService>();
            playerDataService.Data.Reset();
            playerDataService.Commit();
            
            await this.HideAndClose(Bootstrapper.SessionToken);
            await new GotoStateAction(new LobbyState(), false).Execute(Bootstrapper.SessionToken);
        }
    }

    public class CoreCheatsView: UIView<CoreCheatsModel>
    {
        [SerializeField] 
        private Button _looseButton;
        
        [SerializeField] 
        private Button _resetButton;
        
        [SerializeField] 
        private Button _close;

        protected override void OnSetModel()
        {
            _looseButton.onClick.AddListener(() => Model.Loose().Forget());
            _resetButton.onClick.AddListener(() => Model.ResetProgress().Forget());
            _close.onClick.AddListener(() => Model.HideAndClose(Bootstrapper.SessionToken).Forget());
        }
    }
}