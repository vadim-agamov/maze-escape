using JetBrains.Annotations;
using UnityEngine;

namespace Modules.UiComponents
{
    // Adjust sprite scale to fit given RectTransform
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteToRectScaler : MonoBehaviour
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

        [UsedImplicitly]
        public void FitScale()
        {
            var targetCorners = new Vector3[4];
            _targetRect.GetWorldCorners(targetCorners);
            var targetWith = _padding + targetCorners[2].x - targetCorners[0].x;
            var targetHeight = _padding + targetCorners[1].y - targetCorners[0].y;

            var bounds = _spriteRenderer.bounds;
            var widthScale = targetWith / bounds.size.x;
            var heightScale = targetHeight / bounds.size.y;

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
            _spriteRenderer ??= GetComponent<SpriteRenderer>();
        }
    }
}
