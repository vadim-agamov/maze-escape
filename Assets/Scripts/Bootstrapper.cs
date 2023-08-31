using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.FSM;
using Modules.ServiceLocator;
using States;
using Unity.VisualScripting;
using UnityEngine;

public class Bootstrapper
{
    private static CancellationTokenSource _applicationCancellation;

    public static CancellationToken SessionToken => _applicationCancellation.Token;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitializeSceneStart()
    {
        Application.quitting += OnApplicationQuit;
        _applicationCancellation = new CancellationTokenSource();
        Fsm.Enter(new LoadingState(), _applicationCancellation.Token).Forget();
    }

    private static void OnApplicationQuit()
    {
        Application.quitting -= OnApplicationQuit;
        _applicationCancellation.Cancel();
        _applicationCancellation = null;
    }
}