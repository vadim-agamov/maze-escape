using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Maze;
using Maze.MazeService;
using Modules.ServiceLocator;
using Modules.UIService;
using Services;
using Services.CoreService;
using Services.PlayerDataService;
using SN;
using States;
using UI;
using UnityEngine;

namespace Actions
{
    public class LooseLevelAction
    {
        private readonly IPlayerDataService _playerDataService;
        private readonly int _levelScore;
        private readonly IMazeService _mazeService;

        public LooseLevelAction()
        {
            _mazeService = ServiceLocator.Get<IMazeService>();
            _playerDataService = ServiceLocator.Get<IPlayerDataService>();
            _levelScore = _mazeService.Context.Score;
        }

        public async UniTask Execute(CancellationToken token)
        {
            var data = _playerDataService.Data;
            data.MaxScore = Math.Max(data.MaxScore, _levelScore);
            _playerDataService.Commit();

            var model = new LevelLoosedModel()
            {
                Score = _levelScore
            };
            await model.OpenAndShow("LevelLoosedUI", token);
            await model.WaitForHide(token);
            Physics2D.gravity = -Physics2D.gravity;
            // await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: token);
            await SnBridge.Instance.ShowInterstitial();
            await new GotoStateAction(new MazeState(), true).Execute(token);
            Physics2D.gravity = -Physics2D.gravity;
        }
    }
}