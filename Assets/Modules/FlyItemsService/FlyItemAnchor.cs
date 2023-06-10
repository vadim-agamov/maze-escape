using UnityEngine;
using UnityEngine.Events;

namespace Modules.FlyItemsService
{
    public class FlyItemAnchor: MonoBehaviour
    {
        [SerializeField] 
        private string _id;

        [SerializeField] 
        private UnityEvent<int> _onEvent;

        public string Id => _id;

        public void Play(int v) => _onEvent?.Invoke(v); 
        
        private void OnEnable()
        {
            ServiceLocator.ServiceLocator.Get<IFlyItemsService>().RegisterAnchor(this);
        }
        
        private void OnDisable()
        {
            ServiceLocator.ServiceLocator.Get<IFlyItemsService>().UnregisterAnchor(this);
        }
    }
}