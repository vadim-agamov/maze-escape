using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Items
{
    public class Draggable : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private Rigidbody2D _rigidbody2D;
        private Collider2D _collider;
        private Camera _camera;
        private Renderer _renderer;
        private Rect _validArea;
        private bool _drag;
        private Action _onEndDrag;

        public void Initialize(Rect area, Action onEndDrag)
        {
            _validArea = area;
            _onEndDrag = onEndDrag;
            enabled = true;
        }
    
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            _rigidbody2D.velocity = Vector2.zero;
            _drag = true;
            _renderer.sortingOrder = 10;
        }

        private void Update()
        {
            if (!_drag)
                return;
            
            var pos = Input.mousePosition;
            var worldPoint = (Vector2) _camera.ScreenToWorldPoint(pos);
        
            Vector2 size = _collider.bounds.size / 2;
            worldPoint.x = Mathf.Clamp(worldPoint.x, _validArea.xMin + size.x, _validArea.xMax - size.x);
            worldPoint.y = Mathf.Clamp(worldPoint.y, _validArea.yMin + size.y, _validArea.yMax - size.y);
            _rigidbody2D.position = worldPoint;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            var randomAngularVelocity = Random.Range(150, 200);
            _rigidbody2D.angularVelocity = Random.Range(0, 2) > 0 ? randomAngularVelocity : -randomAngularVelocity;
            _drag = false;
            _renderer.sortingOrder = 0;

            _onEndDrag.Invoke();
            
            enabled = false;
        }

        private void Awake()
        {
            _camera = Camera.main;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _renderer = gameObject.GetComponent<Renderer>();
        }
    }
}
