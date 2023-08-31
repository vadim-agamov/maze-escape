using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.UIService.ViewAnimation;
using UnityEngine;

namespace Modules.UIService
{
    public abstract class UIViewBase : MonoBehaviour
    {
        [SerializeReference] 
        private IViewAnimation _showAnimation;
        
        [SerializeReference] 
        private IViewAnimation _hideAnimation;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        public CanvasGroup _canvasGroup;

        protected UIModel BaseModel;
        
        public Animator Animator => _animator;
        public CanvasGroup CanvasGroup => _canvasGroup;
        
        public virtual async UniTask Show(CancellationToken cancellationToken = default)
        {
            gameObject.SetActive(true);
            if (_showAnimation != null)
            {
                await _showAnimation.PlayAsync(this, cancellationToken);
            }
        }

        public virtual async UniTask Hide(CancellationToken cancellationToken = default)
        {
            if (_hideAnimation != null)
            {
                await _hideAnimation.PlayAsync(this, cancellationToken);
            }
            gameObject.SetActive(false);
        }
        
        public void SetModel(UIModel model)
        {
            BaseModel = model;
            OnSetModel();
        }
        
        public void UnsetModel()
        {
            BaseModel = null;
            OnUnsetModel();
        }

        protected virtual void OnSetModel() { }
        internal virtual void OnUpdateModel() { }
        protected virtual void OnUnsetModel() { }
    }

    public abstract class UIView<TModel> : UIViewBase where TModel : UIModel
    {
        protected TModel Model => (TModel)BaseModel;
    }
}