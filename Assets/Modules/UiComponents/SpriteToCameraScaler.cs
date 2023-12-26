using UnityEngine;

namespace Modules.UiComponents
{
    // Adjust sprite scale to fit camera
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteToCameraScaler : MonoBehaviour
    {
        [SerializeField] 
        private Camera _camera;
        
        [SerializeField, HideInInspector]
        private Vector2 _spriteBounds;

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

            var widthScale = targetWith / _spriteBounds.x;
            var heightScale = targetHeight / _spriteBounds.y;

            var scale = Mathf.Max(widthScale, heightScale);
            transform.localScale = new Vector3(scale, scale);
        }

        private void OnValidate()
        {
            _spriteBounds = GetComponent<SpriteRenderer>().bounds.size;
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
