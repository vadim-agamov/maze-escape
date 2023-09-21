namespace Modules.Extensions
{
    public class ValueChangedTrigger<T>
    {
        private T _value;
        
        public T Value => _value;
        
        public bool Changed(T value)
        {
            if (Equals(_value, value))
            {
                return false;
            }

            _value = value;
            return true;
        }
    }
}