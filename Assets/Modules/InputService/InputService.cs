using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Modules.ServiceLocator;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Modules.InputService
{
    public class InputService : IInputService
    {
        private List<RaycastResult> _raycastResult;
        private IInputService This => this;
        private int TouchCount => Input.touchCount > 0 ? Input.touchCount : Input.GetMouseButton(0) ? 1 : 0;
        private PointerEventData _eventDataCurrentPosition;
        
        UniTask IService.Initialize(CancellationToken cancellationToken)
        {
            _raycastResult = new List<RaycastResult>();
            return UniTask.CompletedTask;
        }
        
        void IService.Dispose()
        {
        }

        int IInputService.TouchesCount
        {
            get
            {
                var touchCount = TouchCount;
                if (touchCount > 0)
                {
                    if (CheckPositionOverUI(This.Touch0))
                    {
                        return 0;
                    }
                }

                return touchCount;
            }
        }

        Vector2 IInputService.Touch0 => TouchCount > 1 ? Input.touches[0].position : Input.mousePosition;

        Vector2 IInputService.Touch1 => TouchCount > 1 ? Input.touches[1].position : Input.mousePosition;

        private bool CheckPositionOverUI(Vector2 position)
        {
            if(GUIUtility.hotControl > 0)
                return true;

            _eventDataCurrentPosition ??= new PointerEventData(EventSystem.current);
            _eventDataCurrentPosition.position = position;
            
            EventSystem.current.RaycastAll(_eventDataCurrentPosition, _raycastResult);

            foreach (var result in _raycastResult)
            {
                if (result.module is GraphicRaycaster)
                {
                    return true;
                }
            }

            return false;
        }
    }
}