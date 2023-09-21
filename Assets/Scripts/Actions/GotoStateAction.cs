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
        private IJumpScreenService JumpScreen { get; } = ServiceLocator.Get<IJumpScreenService>();

        public GotoStateAction(IState state, bool withJumpScreen)
        {
            _withJumpScreen = withJumpScreen;
            _state = state;
        }

        public async UniTask Execute(CancellationToken token)
        {
            if (_withJumpScreen)
            {
                await JumpScreen.Show(token);
            }

            await Fsm.Enter(_state, token);

            if (_withJumpScreen)
            {
                await JumpScreen.Hide(token);
            }
        }
    }
}