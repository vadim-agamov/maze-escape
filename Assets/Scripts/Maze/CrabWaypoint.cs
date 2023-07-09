using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Maze
{
    public class CrabWaypoint : MonoBehaviour
    {
        private Tweener _tweener;
        
        public async UniTask Show()
        {
            ResetAnimation();
            gameObject.SetActive(true);
            transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            _tweener = transform.DOScale(1, 1f).SetEase(Ease.OutCubic);
            await _tweener;
        }
        
        public async UniTask Hide()
        {
            ResetAnimation();
            _tweener = transform.DOScale(0.3f, 0.5f).SetEase(Ease.OutQuad);
            await _tweener;
            gameObject.SetActive(false);
        }

        private void ResetAnimation()
        {
            _tweener?.Kill(true);
            _tweener = null;
        }
    }
}