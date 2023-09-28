using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace Modules.UiComponents
{
    // Adjust camera orthographicSize to fit given RectTransform
    [RequireComponent(typeof(Camera))]
    public class CameraSizeFitter: MonoBehaviour
    {
        [HideInInspector]
        [SerializeField] 
        private Camera _camera;

        [SerializeField] 
        private RectTransform _target;

        [SerializeField] 
        private float _padding;

        [SerializeField] 
        private UnityEvent _onFitted;
        
        private void OnEnable()
        {
            FitCameraSize();
        }

        [UsedImplicitly]
        public void FitCameraSize()
        {
            var rect = _target.rect;
            
            // portrait
            var orthographicSizePortrait = rect.height / 2;

            // landscape
            var ratio = Screen.height / (float)Screen.width;
            var orthographicSizeLandscape = ratio * rect.width / 2;

            // The orthographicSize is half the size of the vertical viewing volume.
            _camera.orthographicSize = _padding + Mathf.Max(orthographicSizePortrait, orthographicSizeLandscape);
            
            _onFitted?.Invoke();
        }

        private void OnValidate()
        {
            _camera ??= GetComponent<Camera>();
        }
    }
}