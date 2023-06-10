using Cysharp.Threading.Tasks;
using Modules.BarrierEvents;
using States;

namespace Services.CoreService
{
    public class ScoreTrackerComponent: ICoreComponent
    {
        private CoreContext _context;

        public void Dispose()
        {
            BarrierEvents<ItemsMerged>.Unsubscribe(OnItemSpawned);
        }

        public UniTask Initialize(CoreContext context)
        {
            _context = context;
            BarrierEvents<ItemsMerged>.Subscribe(OnItemSpawned);
            return UniTask.CompletedTask;
        }

        private void OnItemSpawned(ItemsMerged item)
        {
            _context.Score += item.Item.Config.Score;
        }
    }
}