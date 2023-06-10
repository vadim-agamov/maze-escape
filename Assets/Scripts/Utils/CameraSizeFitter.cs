using UnityEngine;

namespace Utils
{
    public class CameraSizeFitter: MonoBehaviour
    {
        [SerializeField] 
        private Camera _camera;

        [SerializeField] 
        private RectTransform _target;

        [SerializeField] 
        private float _padding;

        private void Update()
        {
            // portrait
            var orthographicSizePortrait = _target.rect.height / 2;
            
            // landscape
            var ratio = Screen.height / (float)Screen.width;
            var orthographicSizeLandscape = ratio * _target.rect.width / 2;
            
            // The orthographicSize is half the size of the vertical viewing volume.
            _camera.orthographicSize = _padding + Mathf.Max(orthographicSizePortrait, orthographicSizeLandscape);
        }
    }
}