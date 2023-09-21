using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.UIService;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelWinModel: UIModel
    {
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
    }
    
    public class LevelWinView: UIView<LevelWinModel>
    {
        [SerializeField] 
        private Button _closeButton;
        
        [SerializeField] 
        private Button _playButton;
        
        [SerializeField] 
        private Button _replayButton;
        
        protected override void OnSetModel()
        {
            _closeButton.onClick.AddListener(Model.OnPlay);
            _playButton.onClick.AddListener(Model.OnPlay);
            _replayButton.onClick.AddListener(Model.OnReplay);
        }

        public override async UniTask Hide(CancellationToken cancellationToken = default)
        {
             await base.Hide(cancellationToken);
             Model.OnPlay();
        }
    }
}