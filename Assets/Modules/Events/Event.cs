using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Modules.Events
{
    public static class Event<TEvent> 
    {
        private static readonly HashSet<Action<TEvent>> _subscribers = new HashSet<Action<TEvent>>();
        private static readonly HashSet<Action<TEvent>> _subscribersToAdd = new HashSet<Action<TEvent>>();
        private static readonly HashSet<Action<TEvent>> _subscribersToRemove = new HashSet<Action<TEvent>>();
        private static UniTaskCompletionSource<TEvent> _completionSource;

        public static void Subscribe(Action<TEvent> method)
        {
            _subscribersToAdd.Add(method);
            _subscribersToRemove.Remove(method);
        }

        public static void Unsubscribe(Action<TEvent> method)
        {
            _subscribersToAdd.Remove(method);
            _subscribersToRemove.Add(method);
        }

        public static void Publish(TEvent @event = default)
        {
            if (_subscribersToAdd.Count > 0)
            {
                foreach (var action in _subscribersToAdd)
                {
                    _subscribers.Add(action);
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
                subscriber.Invoke(@event);
            }

            _completionSource?.TrySetResult(@event);
            _completionSource = null;
        }
        
        public static UniTask<TEvent> WaitResult(CancellationToken cancellationToken)
        {
            _completionSource ??= new UniTaskCompletionSource<TEvent>();
            return _completionSource.Task.AttachExternalCancellation(cancellationToken);
        }

        public static UniTask Wait(CancellationToken cancellationToken)
        {
            _completionSource ??= new UniTaskCompletionSource<TEvent>();
            return _completionSource.Task.AttachExternalCancellation(cancellationToken);
        }
    }
}