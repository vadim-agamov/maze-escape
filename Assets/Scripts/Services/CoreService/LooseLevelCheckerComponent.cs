using System.Linq;
using Actions;
using Cysharp.Threading.Tasks;
using Items;
using UI.LooseLevelMarker;
using UnityEngine;
using static System.Single;

namespace Services.CoreService
{
    public class LooseLevelCheckerComponent: MonoBehaviour, ICoreComponent
    {
        [SerializeField]
        private float _warnLevel;

        [SerializeField] 
        private Collider2D _collider2D;

        [SerializeField] 
        private LooseLevelMarker _marker;

        private float _looseTriggeredTime = MaxValue;
        private float _warnTriggeredTime = MaxValue;
        private float _warnTimeout = 1f;
        private float _looseTimeout = 2;
        private Item _looseTriggeredItem;
        private CoreContext _coreContext;
        
#if DEV
        private string _cheatStatus;
#endif
        
        public UniTask Initialize(CoreContext context)
        {
            _marker.Hide();
            _coreContext = context;
            InvokeRepeating(nameof(Check), 1, 0.5f);    
            
            return UniTask.CompletedTask;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(new Vector3(-100, _warnLevel), new Vector3(100, _warnLevel));
        }

        private void Check()
        {
            if(!_coreContext.CanSpawn)
                return;
            
#if DEV
            _cheatStatus = "no";
#endif  
            
            if (CheckWarnItems())
            {
                _warnTriggeredTime = Mathf.Min(Time.time, _warnTriggeredTime);
                var delta = Time.time - _warnTriggeredTime;
                
#if DEV
                _cheatStatus = $"loose warn {delta:F1}";
#endif   
                
                if (delta > _warnTimeout)
                {
                    _marker.Show();
                }
            }
            else
            {
                _warnTriggeredTime = MaxValue;
                _looseTriggeredTime = MaxValue;
                _marker.Hide();
                return;
            }

            if (CheckItems())
            {
                _looseTriggeredTime = Mathf.Min(Time.time, _looseTriggeredTime);
                var delta = Time.time - _looseTriggeredTime;
#if DEV
                _cheatStatus = $"loose in {delta:F1}, id: {_looseTriggeredItem.Id}";
#endif
                if (delta > _warnTimeout)
                {
                    _marker.Show();
                }

                if (delta > _looseTimeout)
                {
                    _marker.Hide();
                    _coreContext.CanSpawn = false;
                    new LooseLevelAction().Execute(Bootstrapper.SessionToken).Forget();
                    CancelInvoke(nameof(Check));
                }
            }
            else
            {
                _looseTriggeredTime = MaxValue;
            }
        }
        
        private bool CheckWarnItems()
        {
            return _coreContext.Items.Any(item => item.transform.position.y < _warnLevel);
        }

        private bool CheckItems()
        {
            if (_looseTriggeredItem != null)
            {
                if(_collider2D.bounds.Intersects(_looseTriggeredItem.Collider.bounds))
                {
                    return true;
                }
                
                _looseTriggeredItem = null;
            }

            foreach (var item in _coreContext.Items)
            {
                if(_collider2D.bounds.Intersects(item.Collider.bounds))
                {
                    _looseTriggeredItem = item;
                    return true;
                }
            }

            return false;
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }
        
#if DEV
        private void OnGUI()
        {
            GUI.Label(new Rect(10, Screen.height - 20, 200, 20), _cheatStatus);
        }
#endif
    }
}