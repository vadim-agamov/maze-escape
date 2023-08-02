using Modules.ServiceLocator;

namespace Services.PlayerDataService
{
    public interface IPlayerDataService : IService
    {
        PlayerData Data { get; }
        void Commit();
    }
}