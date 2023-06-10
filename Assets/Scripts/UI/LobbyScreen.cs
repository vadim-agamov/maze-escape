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
    public class LobbyScreenModel : UIModel
    {
        public void Play()
        {
            new GotoStateAction(new CoreState(), true).Execute(Bootstrapper.SessionToken).Forget();
        }
    }

    public class LobbyScreen : UIView<LobbyScreenModel>
    {
        [SerializeField]
        private Button _playButton;

        protected override void OnSetModel()
        {
            _playButton.onClick.AddListener(Play);
        }
    
        private void Play()
        {
            _playButton.interactable = false;
            Model.Play();
        }
    }
}
