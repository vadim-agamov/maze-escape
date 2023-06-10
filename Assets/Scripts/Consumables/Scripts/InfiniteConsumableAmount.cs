using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Consumables.Scripts
{
    [Serializable]
    public class InfiniteConsumableAmount : IConsumableAmount
    {
        [JsonProperty("duration")] 
        private TimeSpan _duration;

        [JsonProperty("consumable")] 
        public Consumable Consumable;

        [JsonIgnore]
        public TimeSpan Duration
        {
            get => _duration;
            set => _duration = value;
        }

        public InfiniteConsumableAmount(Consumable consumable, TimeSpan durationSeconds)
        {
            Consumable = consumable;
            _duration = durationSeconds;
        }
        
        public string Id => Consumable.Id;
        void IConsumableAmount.Accept(IConsumableVisitor visitor)=> visitor.Visit(this);
        string IConsumableAmount.ToText(ITextFormatter formatter) => formatter.Format(_duration);
        Sprite IConsumableAmount.Icon => Consumable.Icon;

#if DEV
        public override string ToString()
        {
            return $"[{nameof(InfiniteConsumableAmount)}(item={Consumable}, seconds={Duration})]";
        }
#endif
        
    }
}