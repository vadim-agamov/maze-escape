using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using UnityEngine;

namespace Modules.CheatService
{
    public class CheatService: MonoBehaviour, ICheatService
    {
        private bool _isShown;
        private readonly HashSet<ICheatsProvider> _cheatsProviders = new HashSet<ICheatsProvider>();

        private ICheatService This => this;
        
        UniTask IService.Initialize(IProgress<float> progress, CancellationToken cancellationToken)
        {
            Debug.Log($"[{nameof(CheatService)}] Initialize begin");
            DontDestroyOnLoad(gameObject);
            gameObject.name = $"[{nameof(CheatService)}]";
            Debug.Log($"[{nameof(CheatService)}] Initialize end");
            return UniTask.CompletedTask;
        }

        void IService.Dispose()
        {
        }

        void ICheatService.Show() => _isShown = true;

        void ICheatService.Hide() => _isShown = false;

        void ICheatService.RegisterCheatProvider(ICheatsProvider cheatsProvider) => _cheatsProviders.Add(cheatsProvider);

        void ICheatService.UnRegisterCheatProvider(ICheatsProvider cheatsProvider) => _cheatsProviders.Remove(cheatsProvider);

        private void OnGUI()
        {
            GUI.skin.label.fontSize = 40;
            GUI.skin.label.fontStyle = FontStyle.Bold;
            GUI.skin.button.fontSize = 40;
            GUI.skin.textArea.fontSize = 40;
            GUI.skin.textField.fontSize = 40;
            GUI.skin.box.fontSize = 40;
            GUI.skin.box.fontStyle = FontStyle.Bold;
  

            ShowCheats();

            if (_isShown)
            {
                DrawCheats();
            }
        }

        private void Update()
        {
            if (_isShown)
            {
                var touchCount = Input.touchCount > 0 ? Input.touchCount : Input.GetMouseButton(0) ? 1 : 0;
                if (touchCount > 0 &&
                    GUIUtility.hotControl == 0)
                {
                    This.Hide();
                }
            }
        }

        private void DrawCheats()
        {
            GUILayout.BeginArea(new Rect(120, 120, Screen.width - 240, Screen.height - 240));
            foreach (var cheatsProvider in _cheatsProviders)
            {
                GUILayout.BeginVertical(cheatsProvider.Id, "box");
                GUILayout.Space(40);
                cheatsProvider.OnGUI();
                GUILayout.EndVertical();
                GUILayout.Space(10);
            }
            GUILayout.EndArea();
        }

        private void ShowCheats()
        {
            if (GUI.Button(new Rect(Screen.width - 120, Screen.height - 100, 120, 100), "CHTS"))
            {
                if (!_isShown)
                {
                    This.Show();
                }
                else
                {
                    This.Hide();
                }
            }
        }
    }
}