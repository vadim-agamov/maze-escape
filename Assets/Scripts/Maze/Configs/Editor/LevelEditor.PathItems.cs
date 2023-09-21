using System.Collections.Generic;
using System.Linq;
using Modules.Extensions;
using UnityEngine;
using Utils;

namespace Maze.Configs.Editor
{
    public partial class LevelEditor
    {
        private int _pathItems = 3;
        
        private void GeneratePathItems()
        {
            var path = new List<Vector2Int>();
            
            for (var row = 0; row < _cells.GetLength(0); row++)
            {
                for (var col = 0; col < _cells.GetLength(1); col++)
                {
                    if (_cells[row, col].HasFlag(CellType.Path2))
                    {
                        path.Add(new Vector2Int(row, col));
                    }
                }
            }

            var randomItems = path.Shuffle().Take(_pathItems).ToList();
            foreach (var randomItem in randomItems)
            {
                _cells[randomItem.x, randomItem.y] |= CellType.PathItem;
            }
        }
    }
}