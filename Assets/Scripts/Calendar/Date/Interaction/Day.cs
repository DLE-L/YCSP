using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Scripts.Calendar.Todos;
using Scripts.AllData;
using System;

[System.Serializable]
public class DayUi
{
    public TextMeshProUGUI tmp;
    public Image image;
}

namespace Scripts.Calendar.Date.Interaction
{
    public class Day : MonoBehaviour, IPointerDownHandler
    {
        public DayUi dayUi;
        public TodoData todoData;
        public DateTime date;
        void Start()
        {
            dayUi.tmp = GetComponentInChildren<TextMeshProUGUI>();
            dayUi.image = GetComponent<Image>();
        }       

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {            
            TodoUiManager.Instance.StartShowDayTodo(dayUi.tmp);
        }
    }
}
