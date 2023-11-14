using Actions;
using Cysharp.Threading.Tasks;
using Modules.Events;
using Maze.Configs;
using Maze.Events;
using Maze.Service;
using Modules.FlyItemsService;
using Modules.ServiceLocator;
using Modules.UIService;

namespace Maze.Components
{
    public class GoalsCheckerComponent: IComponent
    {
        private Context _context;
        private CharacterController _characterController;
        private IFlyItemsService FlyItemsService { get; } = ServiceLocator.Get<IFlyItemsService>();
        private IUIService UIService { get; } = ServiceLocator.Get<IUIService>();
        
        public UniTask Initialize(Context context, IMazeService mazeService)
        {
            _context = context;
            _characterController = mazeService.GetMazeComponent<CharacterController>();
            return UniTask.CompletedTask;
        }
        
        void IComponent.Update()
        {
            if (_context.Path.Count == 0 || _context.GoalReached)
            {
                return;
            }

            var cell = _context.Path.Last.Value;
            if (cell.CellType.HasFlag(CellType.Finish))
            {
                GoalReached(cell).Forget();
            }
        }

        private async UniTask GoalReached(CellView cell)
        {
            _context.GoalReached = true;
            _context.CurrentGoal++;
            Event<GoalReachedEvent>.Publish(new GoalReachedEvent { Cell = cell });
            if (_context.CurrentGoal >= _context.Goals.Length)
            {
                await WinLevelAction().SuppressCancellationThrow();
            }
            else
            {
                await GoalReachedAction(cell).SuppressCancellationThrow();
            }
            _context.GoalReached = false;
        }

        private async UniTask GoalReachedAction(CellView cell)
        {
            await _characterController.Teleport(Bootstrapper.SessionToken);
            cell.RemoveType(CellType.Finish);
            var nextGoal = _context.Goals[_context.CurrentGoal];
            _context.CellViews[nextGoal.Row, nextGoal.Col].AddType(CellType.Finish);
            
            var screenPoint = _context.Camera.WorldToScreenPoint(cell.transform.position);
            var worldPoint = UIService.Canvas.worldCamera.ScreenToWorldPoint(screenPoint);
            
            FlyItemsService.Fly("goal", worldPoint, "goals_anchor", 1).Forget();
        }

        private async UniTask WinLevelAction()
        {
            _context.Active = false;
            Event<WinLevelEvent>.Publish();
            await _characterController.Teleport(Bootstrapper.SessionToken);
            await new WinLevelAction().Execute(Bootstrapper.SessionToken);
        }

        public void Dispose()
        {
        }
    }
}