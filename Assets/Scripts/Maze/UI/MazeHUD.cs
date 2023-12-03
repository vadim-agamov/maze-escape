using System.Linq;
using JetBrains.Annotations;
using Maze.Components;
using Maze.Service;
using Modules.FlyItemsService;
using Modules.ServiceLocator;
using Modules.SoundService;
using Modules.UIService;
using Modules.Utils;
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

        public int GoalCount => MazeService.Context.Goals.Length * GoalsFactoryComponent.RewardCount;
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
        [SerializeField]
        private Transform _goalsContainer;
        [SerializeField]
        private GameObject _goalPrefab;
        [SerializeField]
        private FlyItemsConfig _flyItemsConfig;

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
            _levelText.text = $"{Model.GoalCount - _completedGoals}";
        }

        [UsedImplicitly]
        public void OnGoalCompleted(string id, int delta)
        {
            _completedGoals+=delta;
            var rewardItem = Instantiate(_goalPrefab, _goalsContainer).GetComponent<Image>();
            rewardItem.name = id;
            rewardItem.sprite = _flyItemsConfig.GetIcon(id);
            rewardItem.preserveAspect = true;
            SortRewards();
            RefreshGoals();
        }
        
        private void SortRewards()
        {
            var unsorted = _goalsContainer.gameObject.GetChildren()
                .OrderBy(go => go.name)
                .ToList();
            
            var sorted = _goalsContainer.gameObject.GetChildren()
                .OrderBy(go => go.name)
                .ToList();

            foreach (var go in unsorted)
            {
                var index = sorted.IndexOf(go);
                go.transform.SetSiblingIndex(index);
            }
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
    