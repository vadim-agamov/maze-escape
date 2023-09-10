using System.Threading;
using Cysharp.Threading.Tasks;
using Maze;
using Modules.ServiceLocator;
using Modules.SocialNetworkService;
using Modules.UIService;
using Services.PlayerDataService;
using UI;

namespace Actions
{
    public class WinLevelAction
    {
        private readonly ISocialNetworkService _snService = ServiceLocator.Get<ISocialNetworkService>();

        public async UniTask Execute(CancellationToken token)
        {
            var model = new LevelWinModel();
            await model.OpenAndShow("LevelWinUI", token);
            
            var action = await model.WaitAction(token);
            if (action == LevelWinModel.LevelWinAction.PlayNext)
            {
                var playerDataService = ServiceLocator.Get<IPlayerDataService>();
                playerDataService.Data.Level++;
                playerDataService.Commit();
            }

            await model.HideAndClose(token);

            await new ShowInterstitialAction().Execute();
            await new GotoStateAction(new MazeState(), true).Execute(token);
        }
    }
}