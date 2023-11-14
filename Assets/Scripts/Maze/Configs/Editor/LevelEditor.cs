using UnityEditor;
using UnityEngine;

namespace Maze.Configs.Editor
{
    public partial class LevelEditor : EditorWindow
    {
        [MenuItem("Game/LevelEditor")]
        private static void OpenWindow() => GetWindow<LevelEditor>().Show();
        
        [SerializeField] 
        private CellType _path = CellType.Path0;
        
        private void GenerateButtonMultipleGoals()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Size");
            _sizeFiled = GUILayout.TextField(_sizeFiled);
            if (int.TryParse(_sizeFiled, out var size))
            {
                _size = size;
            }

            if (GUILayout.Button("Generate"))
            {
                GenerateMaze();
                GenerateGoals();
                _levelConfigObject = null;
                _levelName = $"level_{_size}_{Complexity}_{_cells.GetHashCode():X4}";
            }

            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawBoardItem(Rect rect, CellType cellType)
        {
            var wallColor = Color.white;
            
            var style = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter, 
                fontStyle = FontStyle.Bold,
                fontSize = (int)rect.height - 2,
                normal = new GUIStyleState
                {
                    textColor = Color.white
                }
            };
            
            if (cellType.HasFlag(_path))
            {
                EditorGUI.DrawRect(new Rect(rect), Color.blue);
            }

            if (cellType.HasFlag(CellType.UpWall))
            {
                EditorGUI.DrawRect(new Rect(rect) {height = 1}, wallColor);
            }

            if (cellType.HasFlag(CellType.LeftWall))
            {
                EditorGUI.DrawRect(new Rect(rect) {width = 1}, wallColor);
            }
            
            if (cellType.HasFlag(CellType.RightWall))
            {
                EditorGUI.DrawRect(new Rect(rect) {width = 1, x = rect.xMax-1}, wallColor);
            }
            
            if (cellType.HasFlag(CellType.DownWall))
            {
                EditorGUI.DrawRect(new Rect(rect) {height = 1, y = rect.yMax-1}, wallColor);
            }
            
            if (cellType.HasFlag(CellType.Start))
            {
                EditorGUI.LabelField(rect, "S", style);
            }

            if (cellType.HasFlag(CellType.Finish))
            {
                EditorGUI.LabelField(rect, "F", style);
            }
            
            if (cellType.HasFlag(CellType.PathItem))
            {
                EditorGUI.LabelField(rect, "*", style);
            }
            
            // GUI.FocusControl("");
        }
        
        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.EndHorizontal();

            GenerateButtonMultipleGoals();
            SaveButton();
            LoadButton();
            DrawCells();
        }

        private void DrawCells()
        {
            if(_cells == null)
                return;

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Path0"))
            {
                _path = CellType.Path0;
            }
            if (GUILayout.Button("Path1"))
            {
                _path = CellType.Path1;
            }
            if (GUILayout.Button("Path2"))
            {
                _path = CellType.Path2;
            }
            EditorGUILayout.EndHorizontal();
            
            var rows = _cells.GetLength(0);
            var cols = _cells.GetLength(1);

            var aspectRatio = (float) rows / cols;
            var width = Mathf.CeilToInt(EditorGUIUtility.currentViewWidth);
            var height = Mathf.CeilToInt(EditorGUIUtility.currentViewWidth * aspectRatio);

            var rect = GUILayoutUtility.GetRect(width, height);
            GUI.BeginGroup(rect);
            EditorGUI.DrawRect(new Rect(0,0, width, height), Color.gray);

            var cellsRect = new Rect(10, 10, rect.width - 20, rect.height - 20);
            GUI.BeginGroup(cellsRect);

            var elementSize = (cellsRect.width) / cols;
            var itemRect = new Rect(Vector2.zero, Vector2.one * elementSize);
            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    if (Event.current.type == EventType.MouseDown && itemRect.Contains(Event.current.mousePosition))
                    {
                        _cells[r, c] ^= CellType.PathItem;
                        Debug.Log($"after {_cells[r, c]}");
                    }
                    
                    DrawBoardItem(itemRect, _cells[r, c]);
                    itemRect.x = itemRect.xMax;
                }

                itemRect.x = 0;
                itemRect.y = itemRect.yMax;
            }
            
            GUI.EndGroup();
            GUI.EndGroup();
        }
    }
}