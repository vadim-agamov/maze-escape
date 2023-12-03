using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Maze.Configs;
using Maze.Events;
using Maze.Service;
using Modules.Events;
using Modules.Extensions;
using UnityEngine;

namespace Maze.Components
{
    public class AttentionFxComponent : MonoBehaviour, IComponent
    {
        private IAttentionFxControl _finishFx;
        private IAttentionFxControl _characterFx;
        private Context _context;
        
        UniTask IComponent.Initialize(Context context, IMazeService mazeService)
        {
            Event<BeginWalkEvent>.Subscribe(OnBeginWalk);
            Event<EndWalkEvent>.Subscribe(OnEndWalk);
            Event<WinLevelEvent>.Subscribe(OnWinLevel);
            
            _context = context;
            _characterFx = mazeService.GetMazeComponent<CharacterController>();
            _finishFx = context.CellViews
                .FirstOrDefault(x => x.CellType.HasFlag(CellType.Finish))
                .GetComponent<IAttentionFxControl>();
            
            StartCoroutine(ShowCharacterFx());
            return UniTask.CompletedTask;
        }
        void IComponent.Tick()
        {
            // nothing
        }
        
        private void OnWinLevel(WinLevelEvent _)
        {
            _characterFx.Hide();
            _finishFx.Hide();
            StopAllCoroutines();
        }

        private void OnEndWalk(EndWalkEvent _)
        {
            _characterFx.Hide();
            _finishFx.Hide();
            StopAllCoroutines();
            if (_context.Active)
            {
                StartCoroutine(ShowCharacterFx());
            }
        }

        private void OnBeginWalk(BeginWalkEvent _)
        {
            _characterFx.Hide();
            _finishFx.Hide();
            StopAllCoroutines();
            if (_context.Active)
            {
                StartCoroutine(ShowFinishFx());
            }
        }

        void IDisposable.Dispose()
        {
            Event<BeginWalkEvent>.Unsubscribe(OnBeginWalk);
            Event<EndWalkEvent>.Unsubscribe(OnEndWalk);
            Event<WinLevelEvent>.Unsubscribe(OnWinLevel);
            StopAllCoroutines();
        }

        private IEnumerator ShowCharacterFx()
        {
            yield return new WaitForSeconds(5);
            _characterFx.Show();
        }
        
        private IEnumerator ShowFinishFx()
        {
            yield return new WaitForSeconds(1);
            _finishFx.Show();
        }
    }
}