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
        private static readonly HashSet<IService> _services = new HashSet<IService>();
        
        public static TService Get<TService>() where TService : class, IService
        {
            foreach (var service in _services)
            {
                if (service is TService serviceImplementation)
                    return serviceImplementation;
            }
            
            throw new InvalidOperationException($"Get: Service of type {typeof(TService)} is not registered.");
        }

        public static bool TryGet<TService>(out TService service) where TService : class, IService
        {
            foreach (var currentService in _services)
            {
                if (currentService is TService serviceImplementation)
                {
                    service = serviceImplementation;
                    return true;
                }
            }

            service = default;
            return false;
        }

        public static UniTask RegisterAndInitialize<TService>(TService service, IProgress<float> progress = null, CancellationToken cancellationToken = default) where TService : class, IService
        {
            Register<TService>(service);
            return service.Initialize(progress, cancellationToken);
        }

        public static TService Register<TService>(TService service) where TService: class, IService
        {
#if UNITY_EDITOR
            if (!typeof(TService).IsInterface)
                throw new InvalidOperationException($"Register: Service of type {typeof(TService)} is not interface.");
#endif
            
            if(_services.Any(s => s.GetType() == typeof(TService)))
               // _registered.Any(s => s.GetType() == typeof(TService)))
                throw new InvalidOperationException($"Register: Service of type {typeof(TService)} already registered.");

            Debug.Log($"[{nameof(ServiceLocator)}] Register {typeof(TService)}, {service}");
            _services.Add(service);

            return service;
        }
        
        public static void UnRegister<TService>() where TService : class, IService
        {
            foreach (var service in _services)
            {
                if (service is TService serviceImplementation)
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