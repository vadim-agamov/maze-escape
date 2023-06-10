using System;
using UnityEngine;

namespace Consumables.Scripts
{
    [Serializable]
    public class RewardInfiniteItemConfig : IRewardConfig
    {
        [SerializeField]
        private Consumable _item;

        [SerializeField]
        private int _durationSeconds;

        IConsumableAmount IRewardConfig.Create()
        {
            return new InfiniteConsumableAmount(_item, TimeSpan.FromSeconds(_durationSeconds));
        }
    }
}
