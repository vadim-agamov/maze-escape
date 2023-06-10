using Cysharp.Threading.Tasks;
using Events;
using Maze.MazeService;
using Services.CoreService;
using States;

namespace Maze.Components
{
    public class ScoreTrackerComponent: IComponent
    {
        private Context _context;

        public void Dispose()
        {
            // Event<ItemsMerged>.Unsubscribe(OnItemSpawned);
        }

        public UniTask Initialize(Context context)
        {
            _context = context;
            // Event<ItemsMerged>.Subscribe(OnItemSpawned);
            return UniTask.CompletedTask;
        }

        // private void OnItemSpawned(ItemsMerged item)
        // {
        //     _context.Score += item.Item.Config.Score;
        // }
    }
}