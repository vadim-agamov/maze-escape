using System.Threading;
using Cysharp.Threading.Tasks;

namespace Modules.UIService
{
    public abstract class UIModel
    {
        internal IViewControl ViewControl { get; set; }
        public UniTask Show(CancellationToken token) => ViewControl.Show(token);
        public UniTask Hide(CancellationToken token) => ViewControl.Hide(token);
        public void UpdateModel() => ViewControl.UpdateModel();
    }
}