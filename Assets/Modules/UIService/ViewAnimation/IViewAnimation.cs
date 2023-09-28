using System.Threading;
using Cysharp.Threading.Tasks;

namespace Modules.UIService.ViewAnimation
{
    public interface IViewAnimation
    {
        UniTask PlayAsync(UIView viewBase, CancellationToken cancellationToken);
    }
}