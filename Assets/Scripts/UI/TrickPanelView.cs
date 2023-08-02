using Modules.Events;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Modules;
using Modules.FlyItemsService;
using Modules.ServiceLocator;
using Modules.SoundService;
using Services.PlayerDataService;
using SN;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TrickPanelView: MonoBehaviour
    {
        [SerializeField]
        private Button _useTrick;

        [SerializeField] 
        private TMP_Text _count;
        
        [SerializeField] 
        private GameObject _addLabel;

        [SerializeField] 
        private string _trickId;

        [SerializeField] 
        private GameObject _veil;

        private int _amount;
        private IPlayerDataService _progressService;
        private ISoundService _soundService;
        private Tween _tweener;

        private void OnEnable()
        {
            _progressService = ServiceLocator.Get<IPlayerDataService>();
            _soundService = ServiceLocator.Get<ISoundService>();
            var consumable = _progressService.Data.GetConsumable(_trickId);
            UpdateCount(consumable);

            _useTrick.onClick.AddListener(OnClick);
            // Event<ItemBeginDrag>.Subscribe(Block);
            // Event<ItemEndDrag>.Subscribe(UnBlock);
        }

        private void OnDisable()
        {
            // Event<ItemBeginDrag>.Unsubscribe(Block);
            // Event<ItemEndDrag>.Unsubscribe(UnBlock);
        }

        // private void UnBlock(ItemEndDrag _ = null)
        // {
        //     _useTrick.interactable = true;
        // }
        //
        // private void Block(ItemBeginDrag _ = null)
        // {
        //     _useTrick.interactable = false;
        // }

        private bool NeedBuy
        {
            get
            {
                var availableConsumable = _progressService.Data.GetConsumable(_trickId);
                return !availableConsumable.IsInfinity && availableConsumable.Amount <= 0;
            }
        }
        
        private void OnClick()
        {
            OnClickAsync().Forget();

            async UniTaskVoid OnClickAsync()
            {
                // Block();
                _veil.SetActive(true);
                
                if (NeedBuy)
                {
                    _veil.SetActive(false);
                    await BuyTrick();
                }
                else
                {
                    await UseTrick();
                    _veil.SetActive(false);
                }
                
                // UnBlock();
            }
        }

        private void UpdateCount(PlayerData.Consumable consumable)
        {
            _count.gameObject.SetActive(!NeedBuy);
            _addLabel.gameObject.SetActive(NeedBuy);
            _amount = consumable.Amount; 
            _count.text = _amount.ToString();

            // _tweener?.Kill(true);
            // var sequence = DOTween.Sequence();
            // sequence.Append(
            //         DOTween.To(
            //             () => _amount,
            //             v =>
            //             {
            //                 _amount = v;
            //                 _count.text = _amount.ToString();
            //             },
            //             consumable.Amount,
            //             0.5f)
            //         .SetEase(Ease.Linear));
            // sequence.Insert(0, transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.5f, 2, 0));
            // _tweener = sequence;
        }

        private async UniTask BuyTrick()
        {
            _soundService.Mute();
            var result = await SnBridge.Instance.ShowRewarded();
            _soundService.UnMute();
            if (result == PendingAds.State.CompleteSucceed)
            {
                await ServiceLocator.Get<IFlyItemsService>().Fly(_trickId, "rewards_emitter", _trickId, 5);
                _progressService.Data.AddConsumable(_trickId, 5);
                _progressService.Commit();
            }
        }

        private async UniTask UseTrick()
        {
            var availableConsumable = _progressService.Data.GetConsumable(_trickId);
            if (availableConsumable.IsInfinity)
            {
                await Use();
                return;
            }
            
            var trickUsed = await Use();
            if (trickUsed)
            {
                _progressService.Data.AddConsumable(_trickId, -1);
                _progressService.Commit();
                UpdateCount(-1);
            }

            async UniTask<bool> Use()
            {
                var result = false;
          
                // if (_trickId == "hammer")
                // {
                //     result = await new UseHammerTrickAction().Execute();
                // }
                // else if (_trickId == "swap")
                // {
                //     result = await new UseChangeItemTrickAction().Execute();
                // }
                // else if (_trickId == "rainbow")
                // {
                //     result = await new UseRainbowTrickAction().Execute();
                // }
                // else if (_trickId == "rocket")
                // {
                //     result = await new UseRocketTrickAction().Execute();
                // }
                
                return result;
            }
        }

        public void UpdateCount(int delta)
        {
            _amount += delta;
            _count.gameObject.SetActive(!NeedBuy);
            _addLabel.gameObject.SetActive(NeedBuy);
            _count.text = _amount.ToString();
            _tweener?.Kill(true);
            _tweener = transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.5f, 2, 0);
            if (delta > 0)
            {
                _soundService.Play("tap");
            }
        }
    }
}