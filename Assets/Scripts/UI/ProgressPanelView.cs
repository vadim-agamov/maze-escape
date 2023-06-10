using System.Collections.Generic;
using System.Linq;
using Config;
using Modules.BarrierEvents;
using Modules.ServiceLocator;
using Services;
using States;
using UnityEngine;

namespace UI
{
    public class ProgressPanelView: MonoBehaviour
    {
        [SerializeField] 
        private ItemsConfig _itemsConfig;

        [SerializeField] 
        private Transform _content;

        [SerializeField] 
        private ProgressPanelItem _item;

        private Dictionary<string, ProgressPanelItem> _items = new Dictionary<string, ProgressPanelItem>();
        
        private void OnEnable()
        {
            BarrierEvents<ItemsMerged>.Subscribe(OnItemSpawned);
            
            var progress = ServiceLocator.Get<IPlayerDataService>();
            
            foreach (var itemConfig in _itemsConfig.Items.Where(x=>!x.UnlockedFromStart))
            {
                var item = Instantiate(_item, _content);
                var unlocked = progress.Data.UnlockedItems.Any(x => x == itemConfig.Id);
                item.Setup(itemConfig.Icon, !unlocked);
                _items.Add(itemConfig.Id, item);
            }    
        }

        private void OnItemSpawned(ItemsMerged evt)
        {
            if (_items.TryGetValue(evt.Item.Config.Id, out var item))
            {
                item.Unlock();
            }
        }

        private void OnDisable()
        {
            BarrierEvents<ItemsMerged>.Unsubscribe(OnItemSpawned);
        }
    }
}