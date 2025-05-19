using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Scripts.AllData;
using Scripts.Calendar.Todos;
using UnityEngine.UI;

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
      var dataManager = DataManager.Instance;

      GameObject dayObj = dataManager.itemInfo.day.gameObject;
      List<Day> days = new();
      for (int i = 0; i < TOTAL_DATE_NUM; i++)
      {
        GameObject obj = Instantiate(dayObj, _pnlDay);
        var day = obj.GetComponent<Day>();
        days.Add(day);
      }
      dataManager.Calendar.SetDays(days);
    }

    public void ShowCalendarUi()
    {
      var dataManager = DataManager.Instance;
      CalendarManager calendarManager = dataManager.Calendar;
      DateTime date = dataManager.currentDate;
      List<int> days = calendarManager.GetDaysData(date.Year, date.Month);
      List<Day> scDays = calendarManager.GetDays();

      for (int i = 0; i < TOTAL_DATE_NUM; i++)
      {
        int dayValue = days[i];
        bool isActive = dayValue != 0;

        Day day = scDays[i];
        DayUi dayUi = scDays[i].dayUi;

        day.day = dayValue;

        string newtxt = isActive ? dayValue.ToString() : string.Empty;
        // 기존 Ui 상태와 다를시 변경
        if (dayUi.tmp.text != newtxt) dayUi.tmp.text = newtxt;
        if (day.enabled != isActive) day.enabled = isActive;
      }

      _tmpYear.text = date.Year.ToString();
      _tmpMonth.text = date.Month.ToString("D2");
    }

    // public void UpdateDay(string todoId)
    // {
    //   var dataManager = DataManager.Instance;
    //   TodoManager todoManager = dataManager.Todo;
    //   CalendarManager calendarManager = dataManager.Calendar;
    //   TodoData todoData = todoManager.GetTodoData(todoId);
    //   Day day = calendarManager.GetDay(todoData.EndDate.Date);
    //   //TodoComplete complete = DataManager.Instance.Complete.GetTodoComplete(todoId);

    //   day.gameObject.GetComponent<Image>().color = /*(complete.Complete != 0) ? new Color32(0x32, 0xCD, 0x32, 0xFF) :*/ new Color32(0xff, 0x9a, 0xa2, 255);
    // }

    /// <summary>
    /// 연도, 월, 날짜 변경시 호출출
    /// </summary>
    public void DateUpdate(int month)
    {
      StartCoroutine(StartDateUpdate(month));
    }

    public IEnumerator StartDateUpdate(int month)
    {
      var dataManager = DataManager.Instance;
      DateTime date = dataManager.currentDate;
      if (!dataManager.Calendar.IsDateRange(date.AddMonths(month))) yield break;

      dataManager.currentDate = dataManager.currentDate.AddMonths(month);
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

