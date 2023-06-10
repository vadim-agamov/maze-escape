using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Modules.BarrierEvents
{
    public static class BarrierEvents<T> where T : class
    {
        private static HashSet<Action<T>> _subscribers = new HashSet<Action<T>>();
        private static HashSet<Action<T>> _subscribersToAdd = new HashSet<Action<T>>();
        private static HashSet<Action<T>> _subscribersToRemove = new HashSet<Action<T>>();
        private static UniTaskCompletionSource<T> _completionSource;

        public static void Subscribe(Action<T> method)
        {
            if(!_subscribersToAdd.Contains(method))
                _subscribersToAdd.Add(method);
            
            if (_subscribersToRemove.Contains(method))
                _subscribersToRemove.Remove(method);
        }

        public static void Unsubscribe(Action<T> method)
        {
            if(!_subscribersToAdd.Contains(method))
                _subscribersToAdd.Remove(method);
            
            if (!_subscribersToRemove.Contains(method))
                _subscribersToRemove.Add(method);
        }

        public static void Publish(T args = null)
        {
            if (_subscribersToAdd.Count > 0)
            {
                foreach (var action in _subscribersToAdd)
                {
                    if (!_subscribers.Contains(action))
                    {
                        _subscribers.Add(action);
                    }
                }

                _subscribersToAdd.Clear();
            }

            if (_subscribersToRemove.Count > 0)
            {
                foreach (var action in _subscribersToRemove)
                {
                    _subscribers.Remove(action);
                }

                _subscribersToRemove.Clear();
            }
            
            foreach (var subscriber in _subscribers)
            {
                subscriber.Invoke(args);
            }

            _completionSource?.TrySetResult(args);
            _completionSource = null;
        }
        
        public static UniTask<T> WaitResult(CancellationToken cancellationToken = default)
        {
            if (_completionSource == null)
                _completionSource = new UniTaskCompletionSource<T>();

            return _completionSource.Task.AttachExternalCancellation(cancellationToken);
        }

        public static UniTask Wait(CancellationToken cancellationToken = default)
        {
            if (_completionSource == null)
                _completionSource = new UniTaskCompletionSource<T>();

            return _completionSource.Task.AttachExternalCancellation(cancellationToken);
        }
    }
}