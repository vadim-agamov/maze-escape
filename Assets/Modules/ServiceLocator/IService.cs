using System.Threading;
using Cysharp.Threading.Tasks;

namespace Modules.ServiceLocator
{
    public interface IService
    {
        UniTask Initialize(CancellationToken cancellationToken);
        void Dispose();
    }
}