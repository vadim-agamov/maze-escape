using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using UnityEngine;
using UnityEngine.Pool;

namespace Modules
{
    public interface ISoundService: IService
    {
        void Play(string soundId, bool loop = false);
        void Mute();
        void UnMute();
    }

    public class SoundService: MonoBehaviour, ISoundService
    {
        private ObjectPool<AudioSource> _objectPool;
        private List<AudioSource> _activeSources;
        private CancellationTokenSource _cancellationToken;

        void ISoundService.Play(string soundId, bool loop)
        {
            Play(soundId, loop).Forget();
        }

        private async UniTaskVoid Play(string soundId, bool loop)
        {
            var source = _objectPool.Get();
            if (source.clip == null || 
                source.clip.name != soundId)
            {
                source.clip = Resources.Load<AudioClip>($"Sounds/{soundId}");
            }

            source.loop = loop;
            source.Play();
            
            if (!loop)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(source.clip.length), cancellationToken: _cancellationToken.Token).SuppressCancellationThrow();
                _objectPool.Release(source);
            }
        }

        void ISoundService.Mute()
        {
            foreach (var activeSource in _activeSources)
            {
                activeSource.mute = true;
            }
        }

        void ISoundService.UnMute()
        {
            foreach (var activeSource in _activeSources)
            {
                activeSource.mute = false;
            }
        }

        UniTask IService.Initialize(IProgress<float> progress, CancellationToken cancellationToken)
        {
            DontDestroyOnLoad(gameObject);
            gameObject.name = nameof(SoundService);
            gameObject.AddComponent<AudioListener>();
            _objectPool = new ObjectPool<AudioSource>(OnCreate, OnGet, OnRelease);
            _activeSources = new List<AudioSource>();
            _cancellationToken = new CancellationTokenSource();
            return UniTask.CompletedTask;
        }

        private void OnRelease(AudioSource item)
        {
            _activeSources.Remove(item);
        }

        private void OnGet(AudioSource item)
        {
            _activeSources.Add(item);
        }

        private AudioSource OnCreate()
        {
            var go = new GameObject("AudioSource");
            go.transform.SetParent(gameObject.transform);
            return go.AddComponent<AudioSource>();
        }

        void IService.Dispose()
        {
            _cancellationToken.Cancel();
            
            foreach (var activeSource in _activeSources)
            {
                activeSource.Stop();
                Destroy(activeSource.gameObject);
            }
            
            _objectPool.Dispose();
        }
    }
}