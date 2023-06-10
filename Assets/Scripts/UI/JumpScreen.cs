using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class JumpScreen : MonoBehaviour
    {
        [SerializeField] 
        private GameObject _item;

        [SerializeField]
        private CanvasGroup _canvasGroup;

        private void OnEnable()
        {
            _item.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }

        public async UniTask Show()
        {
            _canvasGroup.alpha = 1;
            _item.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            
            await DOTween.Sequence()
                .Insert(0, _item.transform.DORotate(new Vector3(1f, 1f, 360f), 1f, RotateMode.FastBeyond360))
                .Insert(0, _item.transform.DOScale(new Vector3(50f, 50f, 50f), 1f));
        }
    
        public async UniTask Hide()
        {
            await _canvasGroup.DOFade(0, 0.5f).SetEase(Ease.InQuad);
        }
    }
}
