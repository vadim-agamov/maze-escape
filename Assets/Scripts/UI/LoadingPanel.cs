using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LoadingPanel: MonoBehaviour
    {
        [SerializeField] 
        private TMP_Text _progress;
        
        public float Progress;
        
        private float _visualProgress;

        private void Update()
        {
            _visualProgress = Mathf.Lerp(_visualProgress, Progress, 0.1f*Time.time);
            _progress.text = $"{(100 * _visualProgress):0}%";
        }

        public async UniTask Hide()
        {
            while (Mathf.Abs(Progress - _visualProgress) > 0.01)
            {
                await UniTask.Yield();
            }

            await UniTask.Delay(TimeSpan.FromMilliseconds(500));
            gameObject.SetActive(false);
        }
    }
}