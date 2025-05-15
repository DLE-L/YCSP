using System;
using System.Collections.Generic;
using Scripts.Calendar.Date;
using Scripts.Calendar.Date.Interaction;
using Scripts.Calendar.Todos;
using Scripts.Calendar.Todos.Interaction;
using Scripts.Explain;
using Scripts.Tasks;
using UnityEngine;

namespace Scripts.AllData
{
  /// <summary>
  /// 리소스, 데이터 관련 매니저
  /// </summary>
  public class DataManager : MonoBehaviour
  {
    // 테스트용 구글 시트트
    // 원본 https://docs.google.com/spreadsheets/d/1XCO4tWxuM0OO-ih9mptPGTzUWaDn1GaB-oygUSJLL84/edit?gid=0#gid=0
    // 간소화 https://docs.google.com/spreadsheets/d/1T0KuQ1RUWptv2XVKAl1EZaBXqPD4mfUre1JptMb-pIg/edit?gid=0#gid=0
    public static DataManager Instance;

    public ItemInfo itemInfo;

    public TaskManager Task { get; private set; }
    public TodoManager Todo { get; private set; }
    public CalendarManager Calendar { get; private set; }
    public PoolManager Pool { get; private set; }
    public DateTime currentDate;
    public DateTime Today { get; private set; }

    private void Awake()
    {
      if (Instance == null)
      {
        Instance = this;
        DontDestroyOnLoad(gameObject);
      }
      else
      {
        Destroy(gameObject);
      }
      Task = new TaskManager();
      Todo = new TodoManager();
      Calendar = new CalendarManager();
      Pool = new PoolManager();
      Pool.typeContainer = new Dictionary<Type, GameObject>()
            {
                {typeof(TaskItem), itemInfo.taskItem.gameObject},
                {typeof(TodoItem), itemInfo.todoItem.gameObject},
                {typeof(Todo), itemInfo.todo.gameObject},
                {typeof(Day), itemInfo.day.gameObject}
            };

      Task.Load();
      Todo.Load();
      Calendar.LoadCalendarData();

      currentDate = Today = DateTime.Now;
    }

    public void CompareDate()
    {
      currentDate = (currentDate.Year == Today.Year && currentDate.Month == Today.Month) ? Today : new DateTime(currentDate.Year, currentDate.Month, 1);
    }

    public void SetCanvasRaycast(bool acitve)
    {
      var calendar = CalendarUiManager.Instance;
      var task = TaskUiManager.Instance;
      var explain = ExplainUiManager.Instance;

      calendar.SetCanvasRaycast(acitve);
      task.SetCanvasRaycast(acitve);
      explain.SetCanvasRaycast(acitve);
    }
  }

  [System.Serializable]
  public class ItemInfo
  {
    public TaskItem taskItem;
    public TodoItem todoItem;
    public Todo todo;
    public Day day;
  }

  [System.Serializable]
  public class StartDate
  {
    public string Start;
    public DateTime Date;
    public StartDate(string start)
    {
      string[] date = start.Split('-');
      int year = int.Parse(date[0].Trim());
      int month = int.Parse(date[1].Trim());
      int day = int.Parse(date[2].Trim());
      Date = new(year, month, day);
      Start = start;
    }
  }

  [System.Serializable]
  public class EndDate
  {
    public string End;
    public DateTime Date;
    public EndDate(string end)
    {
      string[] date = end.Split('-');
      int year = int.Parse(date[0].Trim());
      int month = int.Parse(date[1].Trim());
      int day = int.Parse(date[2].Trim());
      Date = new(year, month, day);
      End = end;
    }
  }
}
