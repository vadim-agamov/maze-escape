using System;
using System.Linq;
using Config;
using Cysharp.Threading.Tasks;
using Items;
using Modules.BarrierEvents;
using Modules.ServiceLocator;
using Services;
using Services.CoreService;
using States;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Actions
{
    public class UseMergeItemsTrickAction
    {
        private readonly ICoreService _coreService;
        private ItemsConfig _itemsConfig;
        private readonly IPlayerDataService _progress;

        public UseMergeItemsTrickAction()
        {
            _coreService = ServiceLocator.Get<ICoreService>();
            _progress = ServiceLocator.Get<IPlayerDataService>();
        }

        public async UniTask Execute()
        {
            _itemsConfig = await Addressables.LoadAssetAsync<ItemsConfig>("ItemsConfig");
            while (TryFindPair(out var a, out var b, out var result))
            {
                DestroyItem(a).Forget();
                DestroyItem(b).Forget();
                Spawn(result, a.transform.position);
                
                await UniTask.Delay(TimeSpan.FromMilliseconds(100));
            }

            async UniTask DestroyItem(Item item)
            {
                _coreService.Context.Items.Remove(item);
                await item.PlayExplode(); 
                _coreService.Context.ItemsFactory.ReleaseItem(item);
            }
            
            void Spawn(ItemConfig itemType, Vector3 position)
            {
                var item = _coreService.Context.ItemsFactory.GetItem(itemType.Id);
                item.transform.position = position;
                item.PlaySpawn();
                _coreService.Context.Items.AddFirst(item);

                if (_progress.Data.UnlockedItems.All(x => x != item.Config.Id))
                {
                    _progress.Data.UnlockedItems.Add(item.Config.Id);
                }

                BarrierEvents<ItemsMerged>.Publish(new ItemsMerged(){ Item = item});
            }
        }

        private bool TryFindPair(out Item aItem, out Item bItem, out ItemConfig result)
        {
            var items = _coreService.Context.Items;
            foreach (var a in items)
            {
                foreach (var b in items)
                {
                    if(a.Id == b.Id)
                        continue;
                    
                    if (_itemsConfig.TryGetMergeResult(a.Config, b.Config, out result))
                    {
                        aItem = a;
                        bItem = b;
                        return true;
                    }
                }
            }

            aItem = null;
            bItem = null;
            result = null;
            return false;
        }
    }
}