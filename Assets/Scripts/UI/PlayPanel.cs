using System.Threading;
using Actions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Modules.UIService;
using States;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayModel : UIModel
    {
        public void Play()
        {
            new GotoStateAction(new CoreState(), true).Execute(Bootstrapper.SessionToken).Forget();
        }
    }
    public class PlayPanel : UIView<PlayModel>
    {
        [SerializeField]
        private Button _playButton;
        
        protected override void OnSetModel()
        {
            _playButton.onClick.AddListener(Play);
        }

        public override async UniTask Show(CancellationToken cancellationToken = default)
        {
            await base.Show(cancellationToken);
            var rectTransform = _playButton.GetComponent<RectTransform>();
            rectTransform.pivot = new Vector2(0.5f, 10);
            await rectTransform.DOPivotY(0.5f, 0.5f).SetEase(Ease.OutBack);
        }

        private void Play()
        {
            _playButton.interactable = false;
            Model.Play();
        }
    }
}