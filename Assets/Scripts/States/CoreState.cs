using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Items;
using Modules.ServiceLocator;
using Services.CoreService;
using UnityEngine.SceneManagement;
using IState = Modules.FSM.IState;

namespace States
{
    public class ItemsMerged
    {
        public Item Item;
    }

    public class CoreState : IState
    {
        void IDisposable.Dispose()
        {
        }

        async UniTask IState.Enter(CancellationToken cancellationToken)
        {
            await SceneManager.LoadSceneAsync("Scenes/CoreMaze");
            await ServiceLocator.RegisterAndInitialize<ICoreService>(new CoreService(), cancellationToken: cancellationToken);
        }

        async UniTask IState.Exit(CancellationToken cancellationToken)
        {
            ServiceLocator.UnRegister<ICoreService>();
            await SceneManager.LoadSceneAsync("Scenes/Empty");
        }
    }
}