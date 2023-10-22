using System;

namespace Modules.PlayerDataService
{
    public interface IPropertyProvider<T>
    {
        T Value { get; set; }
    }
    
    public sealed class PropertyProvider<T>: IPropertyProvider<T>
    {
        private readonly Func<T> _getter;
        private readonly Action<T> _setter;
        
        public PropertyProvider(Func<T> getter, Action<T> setter)
        {
            _getter = getter;
            _setter = setter;
        }

        public T Value { get => _getter(); set => _setter(value); }
    }
}