using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ProgressPanelItem: MonoBehaviour
    {
        [SerializeField]
        private Image[] _icons;

        [SerializeField]
        private GameObject _locked;
        
        [SerializeField]
        private GameObject _unlocked;
        
        [SerializeField]
        private GameObject _outline;

        public void Setup(Sprite icon, bool locked)
        {
            foreach (var i in _icons)
            {
                i.sprite = icon;
            }
            
            _locked.SetActive(locked);
            _unlocked.SetActive(!locked);
            _outline.SetActive(false);
        }

        public void Unlock()
        {
            _locked.SetActive(false);
            _unlocked.SetActive(true);
            _outline.SetActive(true);
            transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.5f, 1);
        }
    }
}