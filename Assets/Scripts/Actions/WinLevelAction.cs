using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Maze;
using Modules.AnalyticsService;
using Modules.ServiceLocator;
using Modules.UIService;
using Services.PlayerDataService;
using UI;

namespace Actions
{
    public class WinLevelAction
    {
        private IAnalyticsService AnalyticsService { get; } = ServiceLocator.Get<IAnalyticsService>();
        private IPlayerDataService DataService { get; } = ServiceLocator.Get<IPlayerDataService>();

        public async UniTask Execute(CancellationToken token)
        {
            var model = new LevelWinModel();
            await model.OpenAndShow("LevelWinUI", token);
            
            var action = await model.WaitAction(token);
            if (action == LevelWinModel.LevelWinAction.PlayNext)
            {
                AnalyticsService.TrackEvent("WinLevel", new Dictionary<string, object>
                {
                    {"level", DataService.Data.Level.ToString()}
                });
                DataService.Data.Level++;
                DataService.Commit();
            }

            await model.HideAndClose(token);

            await new ShowInterstitialAction().Execute();
            await new GotoStateAction(new MazeState(), true).Execute(token);
        }
    }
}