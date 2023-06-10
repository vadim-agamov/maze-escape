using System;
using Config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Maze.Components;
using Maze.MazeService;
using Modules;
using Modules.ServiceLocator;
using Services.CoreService;
using UnityEngine;

namespace Items
{
    public class Item : MonoBehaviour
    {
        [SerializeField] 
        private Rigidbody2D _rigidbody2D;
        
        [SerializeField] 
        private ItemConfig _config;
        
        [SerializeField] 
        private ParticleSystem _particleSystem;

        [SerializeField]
        private Collider2D _collider2D;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        
        private static uint _nextId;
        private uint _id;

        private Lazy<ItemsCollidedChecker> _itemsCollidedChecker;
        private ISoundService _soundService;

        public static uint NextId => _nextId++;
        public float Velocity => Vector2.Distance(_rigidbody2D.velocity, Vector2.zero);
        public Rigidbody2D Rigidbody => _rigidbody2D;
        public Collider2D Collider => _collider2D;
        public SpriteRenderer Renderer => _spriteRenderer;
        public uint Id => _id;

        public ItemConfig Config => _config;

        private void OnEnable()
        {
            _itemsCollidedChecker = new Lazy<ItemsCollidedChecker>(() =>
                ServiceLocator.Get<IMazeService>().GetComponent<ItemsCollidedChecker>());
            _soundService = ServiceLocator.Get<ISoundService>();
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            ProcessCollision(col);
        }

        private void ProcessCollision(Collision2D col)
        {
            var other = col.gameObject.GetComponent<Item>();
            if (other == null)
                return;

            if (Id > other.Id)
                _itemsCollidedChecker.Value.OnCollide(this, other).Forget();
        }

        public void PlaySpawn()
        {
            var initialScale = new Vector3(Config.Size, Config.Size, Config.Size);
            transform.localScale = Vector3.zero;
            transform.DOScale(initialScale, 0.5f).SetEase(Ease.OutBack);
        }

        public UniTask PlayExplode(bool sound = true)
        {
            _rigidbody2D.simulated = false;
            _particleSystem.Play();

            if (sound)
            {
                _soundService.Play("pop");
            }

            var duration = _particleSystem.main.duration;

            DOTween.Sequence()
                .Insert(0, _spriteRenderer.DOFade(0f, duration).SetEase(Ease.OutCubic))
                .Insert(0, transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), duration).SetEase(Ease.OutBack));
                
            return UniTask.Delay(TimeSpan.FromSeconds(_particleSystem.main.duration));
        }

        public void SetId(uint id) => _id = id;

        private void OnValidate()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }
}
