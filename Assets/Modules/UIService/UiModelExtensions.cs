using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.Events;
using Modules.UIService.Events;

namespace Modules.UIService
{
    public static class UiModelExtensions
    {
        public static UniTask Open(this UIModel model, string key, CancellationToken token)
        {
            var uiService = ServiceLocator.ServiceLocator.Get<IUIService>();
            return uiService.Open(model, key, token);
        }

        public static async UniTask Show(this UIModel model, CancellationToken cancellationToken)
        {
            await model.Show(cancellationToken);
            Event<UiShowEvent>.Publish(new UiShowEvent(model));
        }

        public static async UniTask OpenAndShow(this UIModel model, string key, CancellationToken token)
        {
            await model.Open(key, token);
            await model.Show(token);
        }

        public static async UniTask Hide(this UIModel model, CancellationToken cancellationToken)
        {
            await model.Hide(cancellationToken);
            Event<UiHideEvent>.Publish(new UiHideEvent(model));
        }

        public static void Close(this UIModel model)
        {
            var uiService = ServiceLocator.ServiceLocator.Get<IUIService>();
            uiService.Close(model);
        }

        public static async UniTask HideAndClose(this UIModel model, CancellationToken cancellationToken)
        {
            await model.Hide(cancellationToken);
            model.Close();
        }
    }
}