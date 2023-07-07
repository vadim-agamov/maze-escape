using UnityEngine;

namespace Utils
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteRectScaler : MonoBehaviour
    {
        [SerializeField] 
        private RectTransform _targetRect;

        [SerializeField] 
        private float _padding;

        [HideInInspector] 
        [SerializeField] 
        private SpriteRenderer _spriteRenderer;

        private void OnEnable()
        {
            FitScale();
        }

        public void FitScale()
        {
            transform.localScale = Vector3.one;

            var targetCorners = new Vector3[4];
            _targetRect.GetWorldCorners(targetCorners);
            var targetWith = _padding + targetCorners[2].x - targetCorners[0].x;
            var targetHeight = _padding + targetCorners[1].y - targetCorners[0].y;
            
            var widthScale = targetWith / _spriteRenderer.bounds.size.x;
            var heightScale = targetHeight / _spriteRenderer.bounds.size.y;

            var scale = Mathf.Max(widthScale, heightScale);
            transform.localScale = new Vector3(scale, scale);
        }
        
        public void OnDrawGizmosSelected()
        {
            var r = GetComponent<Renderer>();
            if (r == null)
                return;
            var bounds = r.bounds;
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(Vector3.zero, bounds.size);
        }

        private void OnValidate()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}
