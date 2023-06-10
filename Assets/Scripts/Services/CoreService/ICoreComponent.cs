using System;
using Cysharp.Threading.Tasks;

namespace Services.CoreService
{
    public interface ICoreComponent: IDisposable
    {
        UniTask Initialize(CoreContext context);
    }
}