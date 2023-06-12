using Actions;
using Cysharp.Threading.Tasks;
using Events;
using Maze.Configs;
using Maze.MazeService;
using Modules.ServiceLocator;
using Services.PlayerDataService;

namespace Maze.Components
{
    public class WinLevelCheckerComponent: IComponent
    {
        private Context _context;
        public UniTask Initialize(Context context)
        {
            _context = context;
            Event<PathUpdatedEvent>.Subscribe(OnPathUpdated);
            return UniTask.CompletedTask;
        }

        private void OnPathUpdated(PathUpdatedEvent pathUpdatedEvent)
        {
            if (pathUpdatedEvent.LastCell.CellType.HasFlag(CellType.Finish))
            {
                _context.Active = false;
                var playerDataService = ServiceLocator.Get<IPlayerDataService>();
                playerDataService.Data.Level++;
                playerDataService.Commit();
                
                new WinLevelAction().Execute(Bootstrapper.SessionToken).Forget();
            }
        }

        public void Dispose()
        {
            Event<PathUpdatedEvent>.Unsubscribe(OnPathUpdated);
        }
    }
}