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
        
        private void DrawBoardItem(Rect rect, CellType cellType)
        {
            var wallColor = Color.white;
            
            var style = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter, 
                fontStyle = FontStyle.Bold,
                fontSize = (int)rect.height - 2,
                normal = new GUIStyleState()
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
                EditorGUI.TextArea(rect, "S", style);
            }

            if (cellType.HasFlag(CellType.Finish))
            {
                EditorGUI.TextArea(rect, "F", style);
            }
            
            if (cellType.HasFlag(CellType.PathItem))
            {
                EditorGUI.TextArea(rect, "*", style);
            }
        }
        
        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            // _levelConfig = (LevelConfig)EditorGUILayout.ObjectField(_levelConfig, typeof(LevelConfig), true);
            EditorGUILayout.EndHorizontal();

            GenerateButton();
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
            // EditorGUI.DrawRect(new Rect(0,0, cellsRect.width, cellsRect.height), Color.blue);

            var elementSize = (cellsRect.width) / cols;
            var itemRect = new Rect(Vector2.zero, Vector2.one * elementSize);
            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
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