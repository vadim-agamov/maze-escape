using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.LocalizationService;
using Modules.ServiceLocator;
using Modules.UIService;
using Services.GamePlayerDataService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelWinModel: UIModel
    {
        private GamePlayerDataService PlayerDataService { get; } = ServiceLocator.Get<GamePlayerDataService>();

        public enum LevelWinAction
        {
            Replay,
            PlayNext
        }
        
        private UniTaskCompletionSource<LevelWinAction> _completionSource = new UniTaskCompletionSource<LevelWinAction>();
        
        public void OnPlay()
        {
            _completionSource?.TrySetResult(LevelWinAction.PlayNext);
            _completionSource = null;
        }

        public void OnReplay()
        {
            _completionSource?.TrySetResult(LevelWinAction.Replay);
            _completionSource = null;
        }

        public UniTask<LevelWinAction> WaitAction(CancellationToken token) =>
            _completionSource.Task.AttachExternalCancellation(token);

        public int Level => PlayerDataService.PlayerData.Level;
    }
    
    public class LevelWinView: UIView<LevelWinModel>
    {
        [SerializeField] 
        private Button _closeButton;
        
        [SerializeField] 
        private Button _playButton;
        
        [SerializeField] 
        private Button _replayButton;
        
        [SerializeField] 
        private Localization _level;
        
        protected override void OnSetModel()
        {
            base.OnSetModel();
            _closeButton.onClick.AddListener(Model.OnPlay);
            _playButton.onClick.AddListener(Model.OnPlay);
            _replayButton.onClick.AddListener(Model.OnReplay);
        }

        protected override async UniTask OnShow(CancellationToken cancellationToken = default)
        {
            _level.SetParameters(Model.Level+1);
            await base.OnShow(cancellationToken);
        }

        protected override async UniTask OnHide(CancellationToken cancellationToken = default)
        {
             await base.OnHide(cancellationToken);
             Model.OnPlay();
        }
    }
}