using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Consumables.Scripts
{
    [Serializable]
    public class ConsumableAmount : IConsumableAmount
    {
        [JsonProperty("count")]
        private int _count;

        [JsonProperty("consumable")] 
        public Consumable Consumable;

        [JsonIgnore]
        public int Count
        {
            get => _count;
            set => _count = value;
        }
        
        public ConsumableAmount(Consumable itemConfig, int count)
        {
            Consumable = itemConfig;
            _count = count;
        }
        
        void IConsumableAmount.Accept(IConsumableVisitor visitor)=> visitor.Visit(this);
        string IConsumableAmount.ToText(ITextFormatter formatter) => formatter.Format(_count);

        public string Id => Consumable.Id;
        Sprite IConsumableAmount.Icon => Consumable.Icon;
        

#if DEV
        public override string ToString()
        {
            return $"[{nameof(ConsumableAmount)}(item={Consumable}, count={_count})]";
        }
#endif
    }
}