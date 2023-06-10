using UnityEngine;

namespace Consumables.Scripts
{
    public interface IConsumableAmount
    {
        void Accept(IConsumableVisitor visitor);
        string ToText(ITextFormatter formatter);
        Sprite Icon { get; }
        string Id { get; }
    }
}