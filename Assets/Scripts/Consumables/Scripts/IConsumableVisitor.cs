namespace Consumables.Scripts
{
    public interface IConsumableVisitor
    {
        void Visit(ConsumableAmount consumable);
        void Visit(InfiniteConsumableAmount consumable);
    }
}