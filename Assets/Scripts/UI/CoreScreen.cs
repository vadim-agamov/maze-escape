using Actions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Modules.BarrierEvents;
using Modules.ServiceLocator;
using Modules.UIService;
using Services;
using States;
using TMPro;
using UnityEngine;

namespace UI
{
    public class CoreScreen: MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _scoreLabel;
        
        [SerializeField]
        private TMP_Text _maxScoreLabel;
        
        private Sequence _tween;
        private int _score;

        private void OnEnable()
        {
            BarrierEvents<ItemsMerged>.Subscribe(OnItemSpawned);
            _maxScoreLabel.text = ServiceLocator.Get<IPlayerDataService>().Data.MaxScore.ToString();
            _scoreLabel.text = 0.ToString();
        }

        private void OnDisable()
        {
            BarrierEvents<ItemsMerged>.Unsubscribe(OnItemSpawned);
        }

        private void OnItemSpawned(ItemsMerged e)
        {
            var score = _score;
            _score += e.Item.Config.Score;
            _tween?.Kill(true);
            _tween = DOTween.Sequence();
            _tween.Append(
                DOTween.To(
                () => score,
                v =>
                {
                    score = v;
                    _scoreLabel.text = score.ToString();
                },
                _score,
                0.5f)
                .SetEase(Ease.OutCubic));
            _tween.Insert(0, _scoreLabel.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0.1f), 0.2f, 2, 0));
        }

#if DEV
        private void OnGUI()
        {
            if (GUI.Button(new Rect(Screen.width - 45, Screen.height - 40, 40, 40), "C"))
            {
                new CoreCheatsModel().OpenAndShow("CheatsUI", Bootstrapper.SessionToken).Forget();
            }
        }
#endif
    }
}