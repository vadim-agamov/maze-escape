using System.Linq;
using Actions;
using Cysharp.Threading.Tasks;
using Modules.Events;
using Maze.Configs;
using Maze.Events;
using Maze.Service;

namespace Maze.Components
{
    public class WinLevelCheckerComponent: IComponent
    {
        private Context _context;
        private CharacterComponent _characterComponent;

        public UniTask Initialize(Context context, IMazeService mazeService)
        {
            _context = context;
            _characterComponent = mazeService.GetComponent<CharacterComponent>();
            Event<PathUpdatedEvent>.Subscribe(OnPathUpdated);
            return UniTask.CompletedTask;
        }

        private void OnPathUpdated(PathUpdatedEvent pathUpdatedEvent)
        {
            if (pathUpdatedEvent.Cells.Last().CellType.HasFlag(CellType.Finish))
            {
                WinLevelAction().SuppressCancellationThrow().Forget();
            }
        }

        private async UniTask WinLevelAction()
        {
            _context.Active = false;
            Event<WinLevelEvent>.Publish();

            await _characterComponent.WaitCharacter(Bootstrapper.SessionToken);
            await new WinLevelAction().Execute(Bootstrapper.SessionToken);
        }

        public void Dispose()
        {
            Event<PathUpdatedEvent>.Unsubscribe(OnPathUpdated);
        }
    }
}