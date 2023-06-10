using Cysharp.Threading.Tasks;
using Modules.UIService;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelLoosedModel: UIModel
    {
        public int Score;
        public void OnClosed()
        {
            this.HideAndClose(Bootstrapper.SessionToken).Forget();
        }
    }

    public class LevelLoosedView: UIView<LevelLoosedModel>
    {
        [SerializeField] 
        private Button _closeHandler;

        [SerializeField]
        private TMP_Text _text;

        protected override void OnSetModel()
        {
            _closeHandler.onClick.AddListener(Model.OnClosed);
            _text.text = Model.Score.ToString();
        }

        protected override void OnUnsetModel()
        {
            _closeHandler.onClick.RemoveAllListeners();
        }
    }
}