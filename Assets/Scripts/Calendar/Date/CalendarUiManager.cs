using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Scripts.AllData;
using Scripts.Calendar.Date.Interaction;
using Scripts.Calendar.Todos;
using UnityEngine.UI;
using Scripts.Calendar.Todos.Interaction;

namespace Scripts.Calendar.Date
{
    /// <summary>
    /// 캘린더 Ui관련 매니저저
    /// </summary>
    public class CalendarUiManager : MonoBehaviour
    {
        public static CalendarUiManager Instance;
        [SerializeField] private RectTransform _pnlDay;
        [SerializeField] private TextMeshProUGUI _tmpYear;
        [SerializeField] private TextMeshProUGUI _tmpMonth;
        [SerializeField] private Canvas _calendarCanvas;
        [SerializeField] private Canvas _taskCanvas;

        private const int TOTAL_DATE_NUM = 42;

        void Awake()
        {
            Instance = this;
            DataManager dataManager = DataManager.Instance;
            for (int i = 0; i < TOTAL_DATE_NUM; i++)
            {
                GameObject obj = dataManager.itemInfo.day.gameObject;
                Instantiate(obj, _pnlDay);
            }

            for (int i = 0; i < TOTAL_DATE_NUM; i++)
            {
                Day day = _pnlDay.GetChild(i).GetComponent<Day>();
                dataManager.days.Add(day);
            }
        }

        public void ShowCalendarUi()
        {
            DataManager dataManager = DataManager.Instance;
            TodoManager todoList = dataManager.Todo;
            DateTime date = dataManager.currentDate;
            List<int> days = dataManager.Calendar.GetDaysData(date.Year, date.Month);
            List<Day> scDays = dataManager.days;

            for (int i = 0; i < TOTAL_DATE_NUM; i++)
            {
                int dayValue = days[i];
                bool isActive = dayValue != 0;

                Day day = scDays[i];
                DayUi dayUi = scDays[i].dayUi;

                // 해당 날짜 변경
                if (isActive)
                {
                    DateTime time = new DateTime(date.Year, date.Month, dayValue);
                    var todoData = day.todoData = todoList.GetTodoDayData(time);
                    day.date = time;
                    if (todoData?.TodoSets?.Count > 0)
                    {
                        int count = 0;
                        foreach (var todoSet in todoData.TodoSets)
                        {
                            count += todoSet.Complete;
                        }

                        if (count == todoData?.TodoSets?.Count) { dayUi.image.color = new Color32(0x32, 0xCD, 0x32, 0xFF); }
                        else { dayUi.image.color = new Color32(0xff, 0x9a, 0xa2, 255); }
                    }
                    else dayUi.image.color = Color.white;
                }

                string newtxt = isActive ? dayValue.ToString() : string.Empty;
                // 기존 Ui 상태와 다를시 변경
                if (dayUi.tmp.text != newtxt) dayUi.tmp.text = newtxt;
                if (dayUi.image.enabled != isActive) dayUi.image.enabled = isActive;
                if (day.enabled != isActive) day.enabled = isActive;
            }

            _tmpYear.text = date.Year.ToString();
            _tmpMonth.text = date.Month.ToString("D2");
        }

        public void ShowDayOnly(Todo todo)
        {
            var dataManager = DataManager.Instance;
            var date = dataManager.Todo.GetTodoDate(todo.todoSet);
            foreach (var day in dataManager.days)
            {
                if (day.date == date)
                {
                    DayUi dayUi = day.dayUi;
                    var todoData = day.todoData;
                    if (todoData?.TodoSets?.Count > 0)
                    {
                        int count = 0;
                        foreach (var todoSet in todoData.TodoSets)
                        {
                            count += todoSet.Complete;
                        }

                        if (count == todoData?.TodoSets?.Count) { dayUi.image.color = new Color32(0x32, 0xCD, 0x32, 0xFF); }
                        else { dayUi.image.color = new Color32(0xff, 0x9a, 0xa2, 255); }
                    }
                    else dayUi.image.color = Color.white;
                }
            }
        }

        /// <summary>
        /// 연도, 월, 날짜 변경시 호출출
        /// </summary>
        public void DateUpdate(int month, int year = 0)
        {
            StartCoroutine(StartDateUpdate(month, year));
        }

        public IEnumerator StartDateUpdate(int month, int year = 0)
        {
            var dataManager = DataManager.Instance;            
            dataManager.currentDate = dataManager.currentDate.AddMonths(month).AddYears(year);
            dataManager.CompareDate();
            yield return TodoUiManager.Instance.TodoItemUpdate();
            ShowCalendarUi();
        }

        public void CloseCalendar()
        {
            _calendarCanvas.enabled = false;
            _taskCanvas.enabled = true;

            var dataManager = DataManager.Instance;
            dataManager.currentDate = dataManager.Today;
        }

        public void SetCanvasRaycast(bool active)
        {   
            _calendarCanvas.GetComponent<GraphicRaycaster>().enabled = active;
        }
    }
}

