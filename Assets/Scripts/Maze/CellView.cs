using Maze.Configs;
using UnityEngine;
using Utils;

namespace Maze
{
    public class CellView: MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _leftWall;
        
        [SerializeField]
        private SpriteRenderer _rightWall;
        
        [SerializeField]
        private SpriteRenderer _upWall;
        
        [SerializeField]
        private SpriteRenderer _downWall;
        
        [SerializeField]
        private GameObject _start;
        
        [SerializeField]
        private GameObject _finish;

        [SerializeField] 
        private Sprite[] _verticalWalls;
        
        [SerializeField] 
        private Sprite[] _horizontalWalls;

        private CellType _cellType;
        private int _row;
        private int _col;
        // public bool IsSelected;
        public CellType CellType => _cellType;
        public int Row => _row;
        public int Col => _col;

        public void Setup(CellType cellType, int r, int c)
        {
            _cellType = cellType;
            _row = r;
            _col = c;

            if(_cellType.HasFlag(CellType.RightWall))
            {
                _rightWall.gameObject.SetActive(true);
                _rightWall.sprite = _verticalWalls.Random();
            }
            
            if(_cellType.HasFlag(CellType.LeftWall))
            {
                _leftWall.gameObject.SetActive(true);
                _leftWall.sprite = _verticalWalls.Random();
            }
            
            if(_cellType.HasFlag(CellType.UpWall))
            {
                _upWall.gameObject.SetActive(true);
                _upWall.sprite = _horizontalWalls.Random();
            }
            
            if(_cellType.HasFlag(CellType.DownWall))
            {
                _downWall.gameObject.SetActive(true);
                _downWall.sprite = _horizontalWalls.Random();
            }
            
            if(_cellType.HasFlag(CellType.Start))
            {
                _start.SetActive(true);
            }
            if(_cellType.HasFlag(CellType.Finish))
            {
                _finish.SetActive(true);
            }
            
            // transform.position = new Vector3(_offset*c, _offset*-r, 0);
            gameObject.name = $"cell_{r}_{c}";
        }
        
        // private void OnDrawGizmos()
        // {
        //     if (IsSelected)
        //     {
        //         Gizmos.color = new Color(1, 0, 0, 0.5f);
        //         Gizmos.DrawCube(transform.position, new Vector3(1, 1, 0));
        //     }
        // }
    }
}