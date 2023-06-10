using System.Linq;
using Actions;
using Config;
using Cysharp.Threading.Tasks;
using Modules.BarrierEvents;
using States;
using UnityEngine.AddressableAssets;

namespace Services.CoreService
{
    public class WinLevelCheckerComponent: ICoreComponent
    {
        private ItemsConfig _itemsConfig;
        private ItemConfig _lastItem;

        public void Dispose()
        {
            BarrierEvents<ItemsMerged>.Unsubscribe(OnItemSpawned);
        }

        public async UniTask Initialize(CoreContext context)
        {
            _itemsConfig = await Addressables.LoadAssetAsync<ItemsConfig>("ItemsConfig");
            _lastItem = _itemsConfig.Items.Last();
            // _lastItem = _itemsConfig.Items.ToArray()[3];
            
            BarrierEvents<ItemsMerged>.Subscribe(OnItemSpawned);
        }

        private void OnItemSpawned(ItemsMerged item)
        {
            if (item.Item.Config.Id == _lastItem.Id)
            {
                new WinLevelAction().Execute(Bootstrapper.SessionToken).Forget();
            }
        }
    }
}