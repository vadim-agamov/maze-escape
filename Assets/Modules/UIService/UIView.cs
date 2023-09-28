using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.UIService.ViewAnimation;
using UnityEngine;

namespace Modules.UIService
{
    public abstract class UIView : MonoBehaviour, IViewControl
    {
        [SerializeReference, SubclassSelector] 
        private IViewAnimation _showAnimation;
        
        [SerializeReference, SubclassSelector] 
        private IViewAnimation _hideAnimation;

        protected UIModel BaseModel;

        protected virtual async UniTask OnShow(CancellationToken cancellationToken = default)
        {
            gameObject.SetActive(true);
            if (_showAnimation != null)
            {
                await _showAnimation.PlayAsync(this, cancellationToken);
            }
        }

        protected virtual async UniTask OnHide(CancellationToken cancellationToken = default)
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
        
        protected virtual void OnSetModel() { }
        protected virtual void OnUpdateModel() { }
        
        #region IViewControl
        UniTask IViewControl.Show(CancellationToken cancellationToken) => OnShow(cancellationToken);
        UniTask IViewControl.Hide(CancellationToken cancellationToken) => OnHide(cancellationToken);
        void IViewControl.UpdateModel() => OnUpdateModel();
        GameObject IViewControl.GameObject => gameObject;
        #endregion
    }

    public abstract class UIView<TModel> : UIView where TModel : UIModel
    {
        protected TModel Model => (TModel)BaseModel;
    }
}