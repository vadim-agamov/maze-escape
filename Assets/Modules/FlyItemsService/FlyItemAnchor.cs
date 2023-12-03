using UnityEngine;
using UnityEngine.Events;

namespace Modules.FlyItemsService
{
    public class FlyItemAnchor: MonoBehaviour
    {
        [SerializeField] 
        private string _id;

        [SerializeField] 
        private UnityEvent<string,int> _onEvent;

        public string Id => _id;

        public void Play(string id, int v) => _onEvent?.Invoke(id, v); 
        
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