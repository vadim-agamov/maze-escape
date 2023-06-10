using System.Linq;
using Config;
using Cysharp.Threading.Tasks;
using Items;
using Modules;
using Modules.BarrierEvents;
using Modules.ServiceLocator;
using States;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Services.CoreService
{
    public class ItemsCollidedChecker: MonoBehaviour, ICoreComponent
    {
        private ItemsConfig _itemsConfig;
        private readonly ItemsMerged _itemsMergedEvent = new ItemsMerged();
        private CoreContext _context;
        private IPlayerDataService _progress;

        public async UniTaskVoid OnCollide(Item a, Item b)
        {
            if(!_context.CanSpawn)
                return;
            
            if(!_context.Items.Contains(a) || !_context.Items.Contains(b))
                return;

            if (!_itemsConfig.TryGetMergeResult(a.Config, b.Config, out var result))
                return;

            _context.Items.Remove(a);
            _context.Items.Remove(b);

            if (a.Velocity < b.Velocity)
                Spawn(result, a.transform.position);
            else
                Spawn(result, b.transform.position);
            

            await (a.PlayExplode(), b.PlayExplode(false));
            _context.ItemsFactory.ReleaseItem(a);
            _context.ItemsFactory.ReleaseItem(b);
        }

        private void Spawn(ItemConfig itemType, Vector3 position)
        {
            var item = _context.ItemsFactory.GetItem(itemType.Id);
            item.transform.position = position;
            item.PlaySpawn();
            _context.Items.AddFirst(item);

            if (_progress.Data.UnlockedItems.All(x => x != item.Config.Id))
            {
                _progress.Data.UnlockedItems.Add(item.Config.Id);
            }

            _itemsMergedEvent.Item = item;
            BarrierEvents<ItemsMerged>.Publish(_itemsMergedEvent);
        }

        public async UniTask Initialize(CoreContext context)
        {
            _context = context;
            _itemsConfig = await Addressables.LoadAssetAsync<ItemsConfig>("ItemsConfig");
            _progress = ServiceLocator.Get<IPlayerDataService>();
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}