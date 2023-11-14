using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Maze.Configs;
using Maze.Events;
using Maze.Service;
using Modules.Events;
using Modules.InputService;
using Modules.ServiceLocator;
using Modules.Utils;
using UnityEngine;

namespace Maze.Components
{
    public class InputComponent: IComponent
    {
        private Context _context;
        private readonly ValueChangedTrigger<bool> _touchTrigger = new ();
        private FieldViewComponent _fieldViewComponent;
        private IInputService _inputService;
        private Camera _camera;
        private CellView LastCell => _context.Path.Count > 0 ? _context.Path.Last.Value : _context.CurrentCell;
        
        UniTask IComponent.Initialize(Context context, IMazeService mazeService)
        {
            _inputService = ServiceLocator.Get<IInputService>();
            _fieldViewComponent = mazeService.GetMazeComponent<FieldViewComponent>();
            _context = context;
            _camera = _context.Camera;
            _context.Path.AddLast(_fieldViewComponent.GetStartCell());
            return UniTask.CompletedTask;
        }

        void IComponent.Update()
        {
            if(_context.GoalReached)
            {
                return;
            }
            
            var touchChanged = _touchTrigger.SetValue(_inputService.TouchesCount > 0);
            
            if (_touchTrigger.Value)
            {
                var hitPosition = _camera.ScreenToWorldPoint(_inputService.Touch0);
                hitPosition = new Vector3(hitPosition.x, hitPosition.y, 0);
                var cellView = _context.Path.FirstOrDefault(cell => Vector3.Distance(cell.transform.position, hitPosition) < 0.5f);
                
                // touch cell in path -> cut loop
                if (touchChanged && cellView != null)
                {
                    AddCellToPath(cellView);
                }
            
                // touch cell not in path -> add cell to path
                else 
                {
                    var cell = GetNearestNeighbour(LastCell, hitPosition);
                    AddCellToPath(cell);
                }
            }
            
            ProcessKeyboardInput();
        }

        void IDisposable.Dispose()
        {
        }
        
        private void ProcessKeyboardInput()
        {
            var lastCell = LastCell;
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (!lastCell.CellType.HasFlag(CellType.UpWall))
                {
                    AddCellToPath(_fieldViewComponent.GetCellView(lastCell.Row - 1, lastCell.Col));
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (!lastCell.CellType.HasFlag(CellType.DownWall))
                {
                    AddCellToPath(_fieldViewComponent.GetCellView(lastCell.Row + 1, lastCell.Col));
                }
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (!lastCell.CellType.HasFlag(CellType.LeftWall))
                {
                    AddCellToPath(_fieldViewComponent.GetCellView(lastCell.Row, lastCell.Col - 1));
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (!lastCell.CellType.HasFlag(CellType.RightWall))
                {
                    AddCellToPath(_fieldViewComponent.GetCellView(lastCell.Row, lastCell.Col + 1));
                }
            }
        }

        private CellView GetNearestNeighbour(CellView cell, Vector3 hitPosition)
        {
            var neighbors = new List<CellView>
            {
                cell
            };

            if (!cell.CellType.HasFlag(CellType.UpWall))
            {
                var r = cell.Row - 1;
                var c = cell.Col;
                AddNeighbor(r, c);
            }
            
            if (!cell.CellType.HasFlag(CellType.DownWall))
            {
                var r = cell.Row + 1;
                var c = cell.Col;
                AddNeighbor(r, c);
            }
            
            if (!cell.CellType.HasFlag(CellType.LeftWall))
            {
                var r = cell.Row;
                var c = cell.Col-1;
                AddNeighbor(r, c);
            }
            
            if (!cell.CellType.HasFlag(CellType.RightWall))
            {
                var r = cell.Row;
                var c = cell.Col + 1;
                AddNeighbor(r, c);
            }
            
            var prevDistance = float.MaxValue;
            var result = neighbors.First();
            foreach (var neighbor in neighbors)
            {
                var distance = Vector3.Distance(neighbor.transform.position, hitPosition);
                if (distance < prevDistance)
                {
                    prevDistance = distance;
                    result = neighbor;
                }
            }

            return result;

            void AddNeighbor(int r, int c)
            {
                if (r >= 0 && r < _context.Cells.GetLength(0) &&
                    c >= 0 && c < _context.Cells.GetLength(1))
                {
                    neighbors.Add(_fieldViewComponent.GetCellView(r,c));
                }
            }
        }

        private void AddCellToPath(CellView cell)
        {
            if (_context.Path.Last?.Value == cell)
            {
                return;
            }

            while (_context.Path.Contains(cell))
            {
                _context.Path.RemoveLast();
            }

            _context.Path.AddLast(cell);
            Event<PathUpdatedEvent>.Publish();
        }
    }
}