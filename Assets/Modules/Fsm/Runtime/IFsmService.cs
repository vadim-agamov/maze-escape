using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;

namespace Modules.FSM
{
    public interface IFsmService: IService
    {
        UniTask Enter(IState state, CancellationToken cancellationToken = default);
        IState CurrentState { get; }
    }
}