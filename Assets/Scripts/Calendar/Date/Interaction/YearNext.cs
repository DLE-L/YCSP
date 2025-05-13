using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Calendar.Date.Interaction
{
    public class YearNext : MonoBehaviour, IPointerDownHandler
    {
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            CalendarUiManager.Instance.DateUpdate(0, 1);
        }
    }
}