using Actions;
using Configs;
using Cysharp.Threading.Tasks;
using Events;
using Maze.MazeService;

namespace Maze.Components
{
    public class WinLevelCheckerComponent: IComponent
    {
        public UniTask Initialize(Context context)
        {
            Event<PathUpdatedEvent>.Subscribe(OnPathUpdated);
            return UniTask.CompletedTask;
        }

        private void OnPathUpdated(PathUpdatedEvent pathUpdatedEvent)
        {
            if (pathUpdatedEvent.LastCell.CellType.HasFlag(CellType.Finish))
            {
                new WinLevelAction().Execute(Bootstrapper.SessionToken).Forget();
            }
        }

        public void Dispose()
        {
            Event<PathUpdatedEvent>.Unsubscribe(OnPathUpdated);
        }
    }
}