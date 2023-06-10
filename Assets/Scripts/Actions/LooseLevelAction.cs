using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using Modules.UIService;
using Services;
using Services.CoreService;
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
        private readonly ICoreService _coreService;

        public LooseLevelAction()
        {
            _coreService = ServiceLocator.Get<ICoreService>();
            _playerDataService = ServiceLocator.Get<IPlayerDataService>();
            _levelScore = _coreService.Context.Score;
        }

        public async UniTask Execute(CancellationToken token)
        {
            _coreService.Context.CanSpawn = false;
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
            await new GotoStateAction(new CoreState(), true).Execute(token);
            Physics2D.gravity = -Physics2D.gravity;
        }
    }
}