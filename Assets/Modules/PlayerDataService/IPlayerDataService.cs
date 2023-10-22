using Modules.ServiceLocator;

namespace Modules.PlayerDataService
{
    public interface IPlayerDataService<TData> : IService where TData : new()
    {
        TData Data { get; }
        void Commit();
        void Reset();
    }
}