using DG.Tweening;
using UnityEngine;

namespace UI.LooseLevelMarker
{
    public class LooseLevelMarker: MonoBehaviour
    {
        [SerializeField] 
        private SpriteRenderer _sprite;

        private Tween _tweener;

        private readonly Color _faded = new Color(1, 1, 1, 0);

        public void Show()
        {
            if(_tweener != null)
                return;
            
            _sprite.color = _faded;
            gameObject.SetActive(true);
            _tweener = _sprite.DOColor(Color.white, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }

        public void Hide()
        {
            _tweener?.Kill();
            _tweener = _sprite.DOColor(_faded, 0.5f)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    _tweener = null;
                });
        }
    }
}