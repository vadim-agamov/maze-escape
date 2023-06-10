using System;

namespace Consumables.Scripts
{
    public interface ITextFormatter
    {
        string Format(int value);
        string Format(TimeSpan value);
    }
}