using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Modules.FSM
{
    public interface IState
    {
        UniTask Enter(CancellationToken cancellationToken = default);
        UniTask Exit(CancellationToken cancellationToken = default);
    }
}