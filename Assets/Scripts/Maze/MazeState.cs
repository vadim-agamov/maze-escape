using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Maze.MazeService;
using Modules.ServiceLocator;
using UnityEngine.SceneManagement;
using IState = Modules.FSM.IState;

namespace Maze
{
    public class MazeState : IState
    {
        void IDisposable.Dispose()
        {
        }

        async UniTask IState.Enter(CancellationToken cancellationToken)
        {
            await SceneManager.LoadSceneAsync("Scenes/CoreMaze");
            await ServiceLocator.RegisterAndInitialize<IMazeService>(new MazeService.MazeService(), cancellationToken: cancellationToken);
        }

        async UniTask IState.Exit(CancellationToken cancellationToken)
        {
            ServiceLocator.UnRegister<IMazeService>();
            await SceneManager.LoadSceneAsync("Scenes/Empty");
        }
    }
}