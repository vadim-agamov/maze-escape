using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Items;
using UnityEngine;

namespace Actions
{
    public class RocketTrick : MonoBehaviour
    {
        public event Action<Item> OnCollide;

        public async UniTask Launch(Vector3 position)
        {
            var bottom = Camera.main.ScreenToWorldPoint(Vector3.zero);
            transform.position = new Vector3(position.x, bottom.y);
            await transform.DOMoveY(10, 3f).SetEase(Ease.InQuad);
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if(col.gameObject.layer != LayerMask.NameToLayer("Field"))
                return;
            
            var item = col.gameObject.GetComponent<Item>();
            if (item != null)
            {
                OnCollide?.Invoke(item);
            }  
        }
    }
}