using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.FSM;
using Modules.ServiceLocator;
using Services.JumpScreenService;


namespace Actions
{
    public class GotoStateAction
    {
        private readonly IState _state;
        private readonly bool _withJumpScreen;
        private readonly IJumpScreenService _jumpScreen;

        public GotoStateAction(IState state, bool withJumpScreen)
        {
            _withJumpScreen = withJumpScreen;
            _state = state;
            _jumpScreen = ServiceLocator.Get<IJumpScreenService>();
        }

        public async UniTask Execute(CancellationToken token)
        {
            if (_withJumpScreen)
            {
                await _jumpScreen.Show(token);
            }

            await Fsm.Enter(_state, token);
            // await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: token);

            if (_withJumpScreen)
            {
                await _jumpScreen.Hide(token);
            }
        }
    }
}