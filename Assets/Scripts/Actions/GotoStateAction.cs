using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.FSM;
using Modules.ServiceLocator;
using UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Actions
{
    public class GotoStateAction
    {
        private readonly IState _state;
        private readonly IFsmService _fsm;
        private readonly bool _withJumpScreen;

        public GotoStateAction(IState state, bool withJumpScreen)
        {
            _withJumpScreen = withJumpScreen;
            _state = state;
            _fsm = ServiceLocator.Get<IFsmService>();
        }

        public async UniTask Execute(CancellationToken token)
        {
            JumpScreen jumpScreen = null;
            if (_withJumpScreen)
            {
                var go = await Addressables.InstantiateAsync("JumpScreen");
                go.name = "JumpScreen";
                GameObject.DontDestroyOnLoad(go);
                jumpScreen = go.GetComponent<JumpScreen>();
                await jumpScreen.Show();
            }

            await _fsm.Enter(_state, token);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: token);

            if (_withJumpScreen)
            {
                await jumpScreen.Hide();
                Addressables.ReleaseInstance(jumpScreen.gameObject);
            }
        }
    }
}