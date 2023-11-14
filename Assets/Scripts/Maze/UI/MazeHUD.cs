using JetBrains.Annotations;
using Maze.Service;
using Modules.ServiceLocator;
using Modules.SoundService;
using Modules.UIService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Maze.UI
{
    public class MazeHUDModel : UIModel
    {
        private ISoundService SoundService { get; } = ServiceLocator.Get<ISoundService>();
        private IMazeService MazeService { get; } = ServiceLocator.Get<IMazeService>();
        
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

        public int GoalCount => MazeService.Context.Goals.Length;
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
        [SerializeField]
        private TMP_Text _levelText;

        private ISoundService _soundService;
        private int _completedGoals;

        protected override void OnSetModel()
        {
            base.OnSetModel();
            _soundButton.onClick.AddListener(ToggleSound);
            SetupSoundButton();  
            RefreshGoals();
        }

        private void RefreshGoals()
        {
            _levelText.text = $"{_completedGoals}/{Model.GoalCount}";
        }

        [UsedImplicitly]
        public void OnGoalCompleted(int delta)
        {
            _completedGoals+=delta;
            RefreshGoals();
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
    