using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Modules.UIService
{
    public interface IViewControl
    {
        UniTask Show(CancellationToken cancellationToken = default);
        UniTask Hide(CancellationToken cancellationToken = default);
        void UpdateModel();
        GameObject GameObject { get; }
    }
}