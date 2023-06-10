using Cysharp.Threading.Tasks;
using Modules.BarrierEvents;
using Modules.FlyItemsService;
using Modules.ServiceLocator;
using States;
using UnityEngine;

namespace Services.CoreService
{
    public class ItemsCollidedSeriesCheckerComponent : MonoBehaviour, ICoreComponent
    {
        private int _count;
        private IPlayerDataService _progressService;

        public void Dispose()
        {
            BarrierEvents<ItemsMerged>.Unsubscribe(OnCollided);
            BarrierEvents<ItemSpawnedEvent>.Unsubscribe(OnItemSpawned);
            Destroy(gameObject);
        }

        public UniTask Initialize(CoreContext context)
        {
            _progressService = ServiceLocator.Get<IPlayerDataService>();
            BarrierEvents<ItemsMerged>.Subscribe(OnCollided);
            BarrierEvents<ItemSpawnedEvent>.Subscribe(OnItemSpawned);
            return UniTask.CompletedTask;
        }

        private void OnItemSpawned(ItemSpawnedEvent _)
        {
            _count = 0;
        }

        private void OnCollided(ItemsMerged itemsMerged)
        {
            _count++;

            if (_count == 3)
            {
                Present("swap", 1);
            }
            if (_count == 4)
            {
                Present("hammer", 1);
            }
            if (_count >= 5)
            {
                Present("rocket", 1);
            }

            void Present(string trick, int amount)
            {
                var screenPosition = Camera.main.WorldToScreenPoint(itemsMerged.Item.transform.position);
                ServiceLocator.Get<IFlyItemsService>().Fly(trick, screenPosition, trick, amount).Forget();
                _progressService.Data.AddConsumable(trick, amount);
            }
        }
        
#if DEV
        private void OnGUI()
        {
            GUI.Label(new Rect(10, Screen.height - 50, 200, 20), $"combo: {_count}");
        }
#endif
    }
}