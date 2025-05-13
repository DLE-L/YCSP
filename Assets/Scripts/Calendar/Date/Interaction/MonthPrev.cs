using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Calendar.Date.Interaction
{
    public class MonthPrev : MonoBehaviour, IPointerDownHandler
    {
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            CalendarUiManager.Instance.DateUpdate(-1);
        }
    }
}