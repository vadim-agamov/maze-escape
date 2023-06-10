using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.FSM;
using Modules.ServiceLocator;
using States;
using UnityEngine;

public class Bootstrapper
{
    private static CancellationTokenSource _applicationCancellation;

    public static CancellationToken SessionToken => _applicationCancellation.Token;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitializeSceneStart()
    {
        _applicationCancellation = new CancellationTokenSource();
        var fsmService = ServiceLocator.Register<IFsmService>(new FsmService());
        fsmService.Enter(new LoadingState()).Forget();
    }
}