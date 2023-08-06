using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Services.JumpScreenService
{
    public class JumpScreenService : IJumpScreenService
    {
        private JumpScreen _jumpScreen;
        
        async UniTask IService.Initialize(IProgress<float> progress, CancellationToken cancellationToken)
        {
            var gameObject = await Addressables.InstantiateAsync("JumpScreen").ToUniTask(cancellationToken: cancellationToken);
            gameObject.name = $"[{nameof(JumpScreenService)}]";
            GameObject.DontDestroyOnLoad(gameObject);

            _jumpScreen = gameObject.GetComponent<JumpScreen>();
        }

        void IService.Dispose()
        {
            Addressables.ReleaseInstance(_jumpScreen.gameObject);
        }

        UniTask IJumpScreenService.Show(CancellationToken cancellationToken) => _jumpScreen.Show(cancellationToken);

        UniTask IJumpScreenService.Hide(CancellationToken cancellationToken) => _jumpScreen.Hide(cancellationToken);
    }
}