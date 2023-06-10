using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using UnityEngine;

namespace Modules.UIService
{
    public interface IUIService: IService
    {
        Canvas RootCanvas { get; }
        Camera Camera { get; }
        UniTask Open<TModel>(TModel model, string key, CancellationToken cancellationToken = default) where TModel : UIModel;
        void Close<TModel>(TModel model) where TModel : UIModel;
        bool SetInteractiveState(bool state);
    }
}