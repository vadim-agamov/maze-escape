using System.Collections.Generic;
using Items;
using Modules.ServiceLocator;

namespace Services.CoreService
{
    public class CoreContext
    {
        public int Score;
        public bool CanSpawn;
        public IItemsFactoryComponent ItemsFactory;
        public LinkedList<Item> Items;
    }

    public interface ICoreService : IService
    {
        T GetComponent<T>() where T : ICoreComponent;
        CoreContext Context { get; }
    }
}