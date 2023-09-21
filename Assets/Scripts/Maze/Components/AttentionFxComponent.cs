using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Maze.Configs;
using Maze.Service;
using Modules.Extensions;
using UnityEngine;
using Utils;

namespace Maze.Components
{
    public class AttentionFxComponent : MonoBehaviour, IComponent
    {
        private IAttentionFxControl _finishFx;
        private IAttentionFxControl _characterFx;
        private CharacterComponent _character;
        private Context _context;
        private readonly ValueChangedTrigger<bool> _characterMovingTrigger = new ValueChangedTrigger<bool>();
        
        UniTask IComponent.Initialize(Context context, IMazeService mazeService)
        {
            _context = context;
            _characterFx = _character = mazeService.GetComponent<CharacterComponent>();
            _finishFx = context.CellViews
                .FirstOrDefault(x => x.CellType.HasFlag(CellType.Finish))
                .GetComponent<IAttentionFxControl>();
            
            StartCoroutine(UpdateFx());
            return UniTask.CompletedTask;
        }

        void IDisposable.Dispose()
        {
            StopAllCoroutines();
        }

        private void Update()
        {
            if (_characterMovingTrigger.Changed(_character.IsWalking))
            {
                StopAllCoroutines();
                StartCoroutine(UpdateFx());
            }
        }

        private IEnumerator UpdateFx()
        {
            _characterFx.Hide();
            _finishFx.Hide();
            
            if (_character.IsWalking)
            {
                yield return new WaitForSeconds(1);
                _characterFx.Hide();
                _finishFx.Show();
            }
            else
            {
                yield return new WaitForSeconds(5);
                _characterFx.Show();
                _finishFx.Hide();
            }
        }
    }
}