using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Modules.ServiceLocator
{
    public interface IService
    {
        UniTask Initialize(IProgress<float> progress = null, CancellationToken cancellationToken = default);
        void Dispose();
    }
}