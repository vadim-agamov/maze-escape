using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace Modules.Pool
{
    public static class PooledCollection<T, TData> 
        where T: class, ICollection<TData>, new()
        where TData: struct
    {
        private static readonly ObjectPool<T> Pool = new(Create, actionOnRelease: OnRelease);
        
        public static T Get() => Pool.Get();
        public static void Release(T list) => Pool.Release(list);
        private static T Create() => new();
        private static void OnRelease(T list) => list.Clear();
    }

    public readonly struct ScopedList<T> : IDisposable
        where T: struct
    {
        private readonly List<T> _list;

        private ScopedList(List<T> list) => _list = list;

        void IDisposable.Dispose() => PooledCollection<List<T>, T>.Release(_list);

        public static ScopedList<T> Create(out List<T> list)
        {
            list = PooledCollection<List<T>, T>.Get();
            return new ScopedList<T>(list);
        }
    }
}