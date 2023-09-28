using JetBrains.Annotations;
using Modules.ServiceLocator;
using Modules.SoundService;
using Modules.UIService;
using UnityEngine;
using UnityEngine.UI;

namespace Maze.UI
{
    public class MazeHUDModel : UIModel
    {
        private ISoundService SoundService { get; } = ServiceLocator.Get<ISoundService>();
        
        public void ToggleSound()
        {
            if (SoundIsMuted)
            {
                SoundService.UnMute();
                SoundService.Play("tap");

            }
            else
            {
                SoundService.Play("tap");
                SoundService.Mute();
            }
        }
        public bool SoundIsMuted => SoundService.IsMuted;
    }

    public class MazeHUD : UIView<MazeHUDModel>
    {
        public const string KEY = "MazeHUD";
        
        [SerializeField]
        private Button _soundButton;
        [SerializeField]
        private Sprite _soundOnSprite;
        [SerializeField]   
        private Sprite _soundOffSprite;

        private ISoundService _soundService;

        protected override void OnSetModel()
        {
            base.OnSetModel();
            _soundButton.onClick.AddListener(ToggleSound);
            SetupSoundButton();  
        }

        [UsedImplicitly]
        public void ToggleSound()
        {
            Model.ToggleSound();
            SetupSoundButton();
        }

        private void SetupSoundButton()
        {
            _soundButton.image.sprite = Model.SoundIsMuted ? _soundOffSprite : _soundOnSprite;
        }
    }
}
    