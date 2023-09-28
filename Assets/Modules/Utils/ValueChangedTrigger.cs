namespace Modules.Utils
{
    public class ValueChangedTrigger<T>
    {
        public T Value { get; private set; }

        public bool SetValue(T value)
        {
            if (Equals(Value, value))
            {
                return false;
            }

            Value = value;
            return true;
        }
    }
}