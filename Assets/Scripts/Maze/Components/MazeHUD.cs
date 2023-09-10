using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Maze.MazeService;
using Modules.ServiceLocator;
using Modules.SoundService;
using UnityEngine;
using UnityEngine.UI;

namespace Maze.Components
{
    public class MazeHUD : MonoBehaviour, IComponent
    {
        [SerializeField]
        private Button _soundButton;
        [SerializeField]
        private Sprite _soundOnSprite;
        [SerializeField]   
        private Sprite _soundOffSprite;

        private Context _context;
        private ISoundService _soundService;

        void IDisposable.Dispose()
        {
        }

        UniTask IComponent.Initialize(Context context, IMazeService mazeService)
        {
            _soundService = ServiceLocator.Get<ISoundService>();
            _context = context;
            _soundButton.onClick.AddListener(ToggleSound);
            SetupSoundButton();
            return UniTask.CompletedTask;
        }
        
        [UsedImplicitly]
        public void ToggleSound()
        {
            if(_soundService.IsMuted)
                _soundService.UnMute();
            else
                _soundService.Mute();
            
            SetupSoundButton();
        }

        private void SetupSoundButton()
        {
            _soundButton.image.sprite = !_soundService.IsMuted ? _soundOffSprite : _soundOnSprite;
        }
    }
}
    