using System;
using Configs;
using UnityEngine;
using Utils;

namespace Core
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
        private Sprite[] _verticalWalls;
        
        [SerializeField] 
        private Sprite[] _horizontalWalls;

        private CellType _cellType;
        private int _row;
        private int _col;
        public bool IsSelected;

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
            
            // transform.position = new Vector3(_offset*c, _offset*-r, 0);
            gameObject.name = $"cell_{r}_{c}";
        }

        public bool Contact(CellView other)
        {
            var c = other._col;
            var r = other._row;

            if (c == _col && r == _row)
                return false;
            
            if (Math.Abs(r - _row) > 1)
                return false;
            
            if (Math.Abs(c - _col) > 1)
                return false;

            if (_row == r)
            {
                if (c > _col)
                    return !_cellType.HasFlag(CellType.RightWall);
                
                if(c < _col)
                    return !_cellType.HasFlag(CellType.LeftWall);
            }
            
            if (_col == c)
            {
                if (r > _row)
                    return !_cellType.HasFlag(CellType.DownWall);
                
                if(r < _row)
                    return !_cellType.HasFlag(CellType.UpWall);
            }

            return false;
        }

        private void OnDrawGizmos()
        {
            if (IsSelected)
            {
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                Gizmos.DrawCube(transform.position, new Vector3(1, 1, 0));
            }
        }
    }
}