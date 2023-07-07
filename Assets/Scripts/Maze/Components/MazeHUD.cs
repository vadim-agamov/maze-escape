using System;
using Cysharp.Threading.Tasks;
using Events;
using Maze.MazeService;
using TMPro;
using UnityEngine;

namespace Maze.Components
{
    public class MazeHUD : MonoBehaviour, IComponent
    {
        [SerializeField] 
        private TMP_Text _pathLenght;

        private Context _context;

        private void SetupPath(int pathMin, int current)
        {
            _pathLenght.text = $"{current} / {pathMin}";
            _pathLenght.color = current > pathMin ? Color.red : Color.white;
        }

        private void OnPathUpdated(PathUpdatedEvent evt)
        {
            SetupPath(_context.Level.MinPath, evt.Cells.Count);
        }

        void IDisposable.Dispose()
        {
            Event<PathUpdatedEvent>.Unsubscribe(OnPathUpdated);
        }

        UniTask IComponent.Initialize(Context context)
        {
            _context = context;
            Event<PathUpdatedEvent>.Subscribe(OnPathUpdated);
            SetupPath(_context.Level.MinPath, 0);
            return UniTask.CompletedTask;
        }
    }
}
    