using UnityEngine;

namespace Utils
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteCameraScaler : MonoBehaviour
    {
        [SerializeField] 
        private Camera _camera;

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
            
            var a = _camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
            var b = _camera.ScreenToWorldPoint(new Vector2(0, 0));
            var targetWith = a.x - b.x;
            var targetHeight = a.y - b.y;

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
