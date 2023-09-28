using UnityEngine;

namespace Modules.UiComponents
{
    // Adjust sprite scale to fit camera
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteToCameraScaler : MonoBehaviour
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
            var a = _camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
            var b = _camera.ScreenToWorldPoint(new Vector2(0, 0));
            var targetWith = a.x - b.x;
            var targetHeight = a.y - b.y;

            var bounds = _spriteRenderer.bounds;
            var widthScale = targetWith / bounds.size.x;
            var heightScale = targetHeight / bounds.size.y;

            var scale = Mathf.Max(widthScale, heightScale);
            transform.localScale = new Vector3(scale, scale);
        }

        private void OnValidate()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnDrawGizmosSelected()
        {
            var r = GetComponent<Renderer>();
            if (r == null)
            {
                return;
            }

            var bounds = r.bounds;
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(Vector3.zero, bounds.size);
        }
    }
}
