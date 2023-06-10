using Cysharp.Threading.Tasks;
using Modules.BarrierEvents;
using Modules.ServiceLocator;
using Services.CoreService;

namespace Actions
{
    public class UseChangeItemTrickAction
    {
        private readonly ICoreService _coreService;

        public UseChangeItemTrickAction()
        {
            _coreService = ServiceLocator.Get<ICoreService>();
        }

        public UniTask<bool> Execute()
        {
            return _coreService.GetComponent<SpawnComponent>().ChangeItem();
        }
    }
}