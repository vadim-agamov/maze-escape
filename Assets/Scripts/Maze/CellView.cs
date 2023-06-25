using System.Collections.Generic;
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
        private SpriteRenderer _downLeftWall;
        
        [SerializeField]
        private SpriteRenderer _downRightWall;
        
        [SerializeField]
        private SpriteRenderer _upLeftWall;
        
        [SerializeField]
        private SpriteRenderer _upRightWall;
        
        [SerializeField]
        private GameObject _start;
        
        [SerializeField]
        private GameObject _finish;

        [SerializeField] 
        private Sprite[] _verticalWalls;
        
        [SerializeField] 
        private Sprite[] _horizontalWalls;
        
        [SerializeField] 
        private Sprite[] _cornerWalls;
        
        private CellType _cellType;
        private int _row;
        private int _col;
        public CellType CellType => _cellType;
        public int Row => _row;
        public int Col => _col;
        
        public void Setup(CellType cellType, int r, int c, HashSet<string> walls)
        {
            _cellType = cellType;
            _row = r;
            _col = c;
            _rightWall.sprite = _verticalWalls.Random();
            _leftWall.sprite = _verticalWalls.Random();
            _upWall.sprite = _horizontalWalls.Random();
            _downWall.sprite = _horizontalWalls.Random();
            
            _downLeftWall.sprite = _cornerWalls.Random();
            _downLeftWall.transform.Rotate(0,0, Random.Range(0, 360));
            
            _downRightWall.sprite = _cornerWalls.Random();
            _downRightWall.transform.Rotate(0,0, Random.Range(0, 360));
            
            _upLeftWall.sprite = _cornerWalls.Random();
            _upLeftWall.transform.Rotate(0,0, Random.Range(0, 360));

            _upRightWall.sprite = _cornerWalls.Random();
            _upRightWall.transform.Rotate(0,0, Random.Range(0, 360));

            if(_cellType.HasFlag(CellType.RightWall))
            {
                if (TryAddWallId(GenerateId(r, c + 0.5f)))
                {
                    _rightWall.gameObject.SetActive(true);
                }

                if(TryAddWallId(GenerateId(r - 0.5f, c + 0.5f)))
                {
                    _upRightWall.gameObject.SetActive(true);
                }
                
                if(TryAddWallId(GenerateId(r + 0.5f, c + 0.5f)))
                {
                    _downRightWall.gameObject.SetActive(true);
                }
            }

            if (_cellType.HasFlag(CellType.LeftWall))
            {
                if (TryAddWallId(GenerateId(r, c - 0.5f)))
                {
                    _leftWall.gameObject.SetActive(true);
                }

                if(TryAddWallId(GenerateId(r - 0.5f, c - 0.5f)))
                {
                    _upLeftWall.gameObject.SetActive(true);
                }
                
                if(TryAddWallId(GenerateId(r + 0.5f, c - 0.5f)))
                {
                    _downLeftWall.gameObject.SetActive(true);
                }
            }

            if(_cellType.HasFlag(CellType.UpWall))
            {
                if (TryAddWallId(GenerateId(r - 0.5f, c)))
                {
                    _upWall.gameObject.SetActive(true);
                }

                if(TryAddWallId(GenerateId(r - 0.5f, c - 0.5f)))
                {
                    _upLeftWall.gameObject.SetActive(true);
                }
                
                if(TryAddWallId(GenerateId(r - 0.5f, c + 0.5f)))
                {
                    _upRightWall.gameObject.SetActive(true);
                }
            }
            
            if(_cellType.HasFlag(CellType.DownWall))
            {
                if (TryAddWallId(GenerateId(r + 0.5f, c)))
                {
                    _downWall.gameObject.SetActive(true);
                }

                if(TryAddWallId(GenerateId(r + 0.5f, c - 0.5f)))
                {
                    _downLeftWall.gameObject.SetActive(true);
                }
                
                if(TryAddWallId(GenerateId(r + 0.5f, c + 0.5f)))
                {
                    _downRightWall.gameObject.SetActive(true);
                }
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
            
            bool TryAddWallId(string id)
            {
                if (walls.Contains(id))
                {
                    return false;
                }

                walls.Add(id);
                return true;
            }

            string GenerateId(float a, float b) => $"({a:F1},{b:F1})";
        }
    }
}