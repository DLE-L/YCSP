using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Tasks.Interaction
{
    public class ErrorPopup : MonoBehaviour, IPointerDownHandler
    {
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            TaskUiManager.Instance.CloseErrorPopup();
        }
    }
}