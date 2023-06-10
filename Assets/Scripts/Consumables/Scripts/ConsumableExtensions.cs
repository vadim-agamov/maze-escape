using System.Collections.Generic;
using System.Linq;

namespace Consumables.Scripts
{
    public static class ConsumableExtensions
    {
        private class ConsumableMerger : IConsumableVisitor
        {
            private readonly Dictionary<string, ConsumableAmount> _consumables = new Dictionary<string, ConsumableAmount>();
            private readonly Dictionary<string, InfiniteConsumableAmount> _infiniteConsumables = new Dictionary<string, InfiniteConsumableAmount>();
        
            void IConsumableVisitor.Visit(ConsumableAmount consumable)
            {
                if (_consumables.TryGetValue(consumable.Id, out var item))
                {
                    item.Count += consumable.Count;
                }
                else
                {
                    var newItem = new ConsumableAmount(consumable.Consumable, consumable.Count);
                    _consumables.Add(consumable.Id, newItem);
                }
            }

            void IConsumableVisitor.Visit(InfiniteConsumableAmount consumable)
            {
                if (_infiniteConsumables.TryGetValue(consumable.Id, out var item))
                {
                    item.Duration = item.Duration.Add(consumable.Duration);
                }
                else
                {
                    var newItem = new InfiniteConsumableAmount(consumable.Consumable, consumable.Duration);
                    _infiniteConsumables.Add(consumable.Id, newItem);
                }
            }

            public List<IConsumableAmount> Merge(IEnumerable<IConsumableAmount> consumables)
            {
                foreach (var consumable in consumables)
                {
                    consumable.Accept(this);
                }
            
                var result = new List<IConsumableAmount>(_infiniteConsumables.Count + _consumables.Count);
                result.AddRange(_infiniteConsumables.Values);
                result.AddRange(_consumables.Values);
                return result;
            }
        }
        
        public static List<IConsumableAmount> Merge(this IEnumerable<IConsumableAmount> self)
        {
            return new ConsumableMerger().Merge(self);
        }
        
        public static List<IConsumableAmount> Create(this IEnumerable<IRewardConfig> rewards)
        {
            return rewards.Select(reward => reward.Create()).ToList();
        }
    }
}