using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using Modules.UIService;
using Services.CoreService;
using UI;

namespace Actions
{
    public class WinLevelAction
    {
        private readonly ICoreService _coreService;

        public WinLevelAction()
        {
            _coreService = ServiceLocator.Get<ICoreService>();
        }

        public async UniTask Execute(CancellationToken token)
        {
            _coreService.Context.CanSpawn = false;

            var model = new LevelWinModel();
            await model.OpenAndShow("LevelWinUI", token);
            await model.WaitForHide(token);

            _coreService.Context.CanSpawn = true;
        }
    }
}