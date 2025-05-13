using UnityEngine;
using UnityEngine.EventSystems;
using Scripts.AllData;

namespace Scripts.Calendar.Date.Interaction
{
    public class CloseCalendar : MonoBehaviour, IPointerDownHandler
    {
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {            
            var dataManager = DataManager.Instance;            
            CalendarUiManager.Instance.CloseCalendar();
            dataManager.ConnectTaskAndTodo(null);            
        }
    }
}