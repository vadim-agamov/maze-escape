using System.Threading;
using Cysharp.Threading.Tasks;
using Maze;
using Maze.MazeService;
using Modules.ServiceLocator;
using Modules.UIService;
using Services.CoreService;
using UI;

namespace Actions
{
    public class WinLevelAction
    {
        private readonly IMazeService _mazeService;

        public WinLevelAction()
        {
            _mazeService = ServiceLocator.Get<IMazeService>();
        }

        public async UniTask Execute(CancellationToken token)
        {
            var model = new LevelWinModel();
            await model.OpenAndShow("LevelWinUI", token);
            await model.WaitForHide(token);
            
            await new GotoStateAction(new MazeState(), true).Execute(token);
        }
    }
}