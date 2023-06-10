using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.FSM;
using Modules.UIService;
using SN;
using SN.FbBridge;
using UI;
using UnityEngine.AddressableAssets;

namespace States
{
    public class LobbyState : IState
    {
        private LobbyScreenModel _lobbyScreenModel;

        void IDisposable.Dispose()
        {
        }

        async UniTask IState.Enter(CancellationToken cancellationToken)
        {
            await Addressables.LoadSceneAsync("LobbyScene").ToUniTask(cancellationToken: cancellationToken);
            _lobbyScreenModel = new LobbyScreenModel();
            await _lobbyScreenModel.OpenAndShow("LobbyScreen", cancellationToken);
        }

        async UniTask IState.Exit(CancellationToken cancellationToken)
        {
            await _lobbyScreenModel.Hide(cancellationToken);
            _lobbyScreenModel.Close();
        }
    }
}