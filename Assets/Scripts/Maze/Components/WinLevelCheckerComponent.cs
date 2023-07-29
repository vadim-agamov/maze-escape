using Actions;
using Cysharp.Threading.Tasks;
using Modules.Events;
using Maze.Configs;
using Maze.MazeService;
using Modules.ServiceLocator;
using Services.PlayerDataService;

namespace Maze.Components
{
    public class WinLevelCheckerComponent: IComponent
    {
        private Context _context;
        private PathComponent _pathComponent;
        private CharacterComponent _characterComponent;

        public UniTask Initialize(Context context)
        {
            _context = context;
            var service = ServiceLocator.Get<MazeService.MazeService>();
            _pathComponent = service.GetComponent<PathComponent>();
            _characterComponent = service.GetComponent<CharacterComponent>();
            Event<PathUpdatedEvent>.Subscribe(OnPathUpdated);
            return UniTask.CompletedTask;
        }

        private void OnPathUpdated(PathUpdatedEvent pathUpdatedEvent)
        {
            if (pathUpdatedEvent.Cells.Last.Value.CellType.HasFlag(CellType.Finish))
            {
                WinLevelAction().Forget();
            }
        }

        private async UniTask WinLevelAction()
        {
            _context.Active = false;
            
            var playerDataService = ServiceLocator.Get<IPlayerDataService>();
            playerDataService.Data.Level++;
            playerDataService.Commit();

            await UniTask.WaitUntil(() => _characterComponent.IsWalking, cancellationToken: Bootstrapper.SessionToken);
            // await UniTask.Delay(TimeSpan.FromSeconds(2));
            await new WinLevelAction().Execute(Bootstrapper.SessionToken);
        }

        public void Dispose()
        {
            Event<PathUpdatedEvent>.Unsubscribe(OnPathUpdated);
        }
    }
}