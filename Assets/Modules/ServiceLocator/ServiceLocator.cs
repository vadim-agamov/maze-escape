using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Modules.ServiceLocator
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<IService, UniTaskCompletionSource> _services = new();
        
        public static TService Get<TService>() where TService : class, IService
        {
            foreach (var (service, taskCompletionSource) in _services)
            {
                if (service is TService serviceImplementation)
                {
                    if (taskCompletionSource.Task.Status == UniTaskStatus.Succeeded)
                        return serviceImplementation;

                    throw new InvalidOperationException($"Get: Service of type {typeof(TService)} is not initialized.");
                }
            }

            throw new InvalidOperationException($"Get: Service of type {typeof(TService)} is not registered.");
        }
        
        public static async UniTask<TService> GetAsync<TService>(CancellationToken token) where TService : class, IService
        {
            foreach (var (service, taskCompletionSource) in _services)
            {
                if (service is TService serviceImplementation)
                {
                    if (taskCompletionSource.Task.Status == UniTaskStatus.Succeeded)
                    {
                        return serviceImplementation;
                    }

                    try
                    {
                        await taskCompletionSource.Task.AttachExternalCancellation(token);
                    }
                    catch (OperationCanceledException canceledException)
                    {
                        throw new InvalidOperationException($"Get: Service of type {typeof(TService)} is not initialized.", canceledException);
                    }
                    
                    return serviceImplementation;
                }
            }
            
            throw new InvalidOperationException($"Get: Service of type {typeof(TService)} is not registered.");
        }
        
        public static async UniTask Register<TService>(TService service, CancellationToken cancellationToken = default) where TService : class, IService
        {
#if UNITY_EDITOR
            if (!typeof(TService).IsInterface)
                throw new InvalidOperationException($"Register: Service of type {typeof(TService)} is not interface.");
#endif
            
            if(_services.Any(s => s.Key.GetType() == typeof(TService)))
                throw new InvalidOperationException($"Register: Service of type {typeof(TService)} already registered.");

            var taskCompletionSource = new UniTaskCompletionSource();
            _services.Add(service, taskCompletionSource);
            AwaitServiceInitialization().Forget();
            await taskCompletionSource.Task;

            async UniTask AwaitServiceInitialization()
            {
                try
                {
                    await service.Initialize(cancellationToken);
                    taskCompletionSource.TrySetResult();
                }
                catch (OperationCanceledException _)
                {
                    taskCompletionSource.TrySetCanceled();
                }
                catch (Exception e)
                {
                    taskCompletionSource.TrySetException(e);
                }    
            }
        }
        
        public static void UnRegister<TService>() where TService : class, IService
        {
            foreach (var (service, _) in _services)
            {
                if (service is TService _)
                {
                    Debug.Log($"[{nameof(ServiceLocator)}] UnRegister {typeof(TService)}, {service}");
                    service.Dispose();
                    _services.Remove(service);
                    return;
                }
            }
            
            throw new InvalidOperationException($"UnRegister: No service of type {typeof(TService)}.");
        }
    }
}