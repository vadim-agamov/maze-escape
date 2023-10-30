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
        private class ServiceItem
        {
            public IService Service;
            public bool IsReady;
        }
        
        private static readonly HashSet<ServiceItem> _services = new();
        
        public static TService Get<TService>() where TService : class, IService
        {
            foreach (var serviceItem in _services)
            {
                if (serviceItem.Service is TService serviceImplementation)
                {
                    if (!serviceItem.IsReady)
                    {
                        throw new InvalidOperationException($"[{nameof(ServiceLocator)}] Get: Service of type {typeof(TService).Name} is not ready.");
                    }
                    
                    return serviceImplementation;
                }
            }

            throw new InvalidOperationException($"[{nameof(ServiceLocator)}] Get: Service of type {typeof(TService).Name} is not registered.");
        }
        
        public static async UniTask Register<TService>(TService service, CancellationToken token, params Type[] dependency) where TService : class, IService
        {
            if (_services.Any(s => s.Service.GetType() == typeof(TService)))
            {
                throw new InvalidOperationException($"[{nameof(ServiceLocator)}] Register: Service of type {typeof(TService)} already registered.");
            }

            var serviceItem = new ServiceItem { Service = service, IsReady = false };
            _services.Add(serviceItem);
            
            if (dependency.Any())
            {
                var services = new List<ServiceItem>();
                foreach (var dep in dependency)
                {
                    foreach (var item in _services)
                    {
                        var type = item.Service.GetType();
                        if (dep.IsAssignableFrom(type))
                        {
                            services.Add(item);
                        }
                    }
                }
                
                await UniTask.WaitWhile(() => services.Any(s => !s.IsReady), cancellationToken: token);
            }
            
            Debug.Log($"[{nameof(ServiceLocator)}] Initialize begin {typeof(TService).Name}");
            await service.Initialize(token);
            serviceItem.IsReady = true;
            Debug.Log($"[{nameof(ServiceLocator)}] Initialize end {typeof(TService).Name}");
        }
        
        public static void UnRegister<TService>() where TService : class, IService
        {
            foreach (var service in _services)
            {
                if (service.Service is TService _)
                {
                    Debug.Log($"[{nameof(ServiceLocator)}] UnRegister {typeof(TService).Name}, {service}");
                    service.Service.Dispose();
                    _services.Remove(service);
                    return;
                }
            }
            
            throw new InvalidOperationException($"[{nameof(ServiceLocator)}] UnRegister: No service of type {typeof(TService).Name}");
        }
    }
}