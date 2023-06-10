using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Consumables.Scripts
{
    [Serializable]
    public class RandomCountRewardItemConfig : IRewardConfig
    {
        [SerializeField]
        private Consumable _item;
        
        [SerializeField] 
        private int _countMin;
        
        [SerializeField] 
        private int _countMax;

        IConsumableAmount IRewardConfig.Create()
        {
            return new ConsumableAmount(_item, Random.Range(_countMin, _countMax));
        }
    }
}