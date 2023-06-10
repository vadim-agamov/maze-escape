using System;
using UnityEngine;

namespace Consumables.Scripts
{
    [Serializable]
    public class RewardItemConfig : IRewardConfig
    {
        [SerializeField]
        private Consumable _item;

        [SerializeField]
        private int _count;

        public Consumable ItemConfig => _item;
        public int Count => _count;

        IConsumableAmount IRewardConfig.Create() => new ConsumableAmount(_item, _count);
        
        public ConsumableAmount CreateConsumableAmount() => new ConsumableAmount(_item, _count);
    }
}
