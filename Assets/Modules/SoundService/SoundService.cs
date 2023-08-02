using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using UnityEngine;
using UnityEngine.Pool;

namespace Modules.SoundService
{
    public class SoundService: MonoBehaviour, ISoundService
    {
        private ObjectPool<AudioSource> _objectPool;
        private List<AudioSource> _activeSources;
        private CancellationTokenSource _cancellationToken;
        private bool _isMuted;

        UniTask IService.Initialize(IProgress<float> progress, CancellationToken _)
        {
            DontDestroyOnLoad(gameObject);
            gameObject.name = nameof(SoundService);
            gameObject.AddComponent<AudioListener>();
            _objectPool = new ObjectPool<AudioSource>(OnCreate, OnGet, OnRelease, OnDestroyAudioSource);
            _activeSources = new List<AudioSource>();
            _cancellationToken = new CancellationTokenSource();
            return UniTask.CompletedTask;
        }

        void IService.Dispose()
        {
            _cancellationToken.Cancel();
            _objectPool.Dispose();
            Destroy(this);
        }

        void ISoundService.Play(string soundId, bool loop)
        {
            Play(soundId, loop).Forget();
        }

        void ISoundService.Stop(string soundId)
        {
            foreach (var audioSource in _activeSources)
            {
                if (audioSource.clip.name == soundId)
                {
                    audioSource.Stop();
                    _objectPool.Release(audioSource);
                    break;
                }
            }
        }

        void ISoundService.Mute()
        {
            _isMuted = true;
            foreach (var activeSource in _activeSources)
            {
                activeSource.mute = true;
            }
        }

        void ISoundService.UnMute()
        {
            _isMuted = false;
            foreach (var activeSource in _activeSources)
            {
                activeSource.mute = false;
            }
        }

        bool ISoundService.IsMuted => _isMuted;

        private async UniTask Play(string soundId, bool loop)
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
                await UniTask.Delay(TimeSpan.FromSeconds(source.clip.length), cancellationToken: _cancellationToken.Token);
                _objectPool.Release(source);
            }
        }

        private AudioSource OnCreate()
        {
            var go = new GameObject("AudioSource");
            go.transform.SetParent(gameObject.transform);
            return go.AddComponent<AudioSource>();
        }

        private void OnDestroyAudioSource(AudioSource item)
        {
            item.Stop();
            Destroy(item.gameObject);
        }

        private void OnRelease(AudioSource item) => _activeSources.Remove(item);

        private void OnGet(AudioSource item) => _activeSources.Add(item);
    }
}