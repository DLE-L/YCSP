using System;
using System.Collections.Generic;
using System.Globalization;
using Scripts.Calendar.Date;
using Scripts.Calendar.Todos;
using Scripts.Calendar.Todos.Interaction;
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
    // https://docs.google.com/spreadsheets/d/1T0KuQ1RUWptv2XVKAl1EZaBXqPD4mfUre1JptMb-pIg/edit?gid=713577195#gid=713577195
    public static DataManager Instance;

    public ItemInfo itemInfo;

    public TaskManager Task { get; private set; }
    public TodoManager Todo { get; private set; }
    public TodoCompleteManager Complete { get; private set; }
    public CalendarManager Calendar { get; private set; }
    public PoolManager Pool { get; private set; }
    public DateTime Today { get; private set; }
    public DateTime currentDate;    

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
      Complete = new TodoCompleteManager();
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
      Complete.Load();
      Calendar.Load();     

      currentDate = Today = DateTime.Now.Date;
    }

    public void SetCanvasRaycast(bool acitve)
    {
      var calendar = CalendarUiManager.Instance;
      var task = TaskUiManager.Instance;
      //var explain = ExplainUiManager.Instance;

      calendar.SetCanvasRaycast(acitve);
      task.SetCanvasRaycast(acitve);
      //explain.SetCanvasRaycast(acitve);
    }

    public void CompareDate()
    {
      currentDate = (currentDate.Month == Today.Month && currentDate.Year == Today.Year) ? Today : new DateTime(currentDate.Year, currentDate.Month, 1);
    }

    public void SetCompleteData(string todoId)
    {

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
    public DateTime Date => DateTime.ParseExact(Start, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    public StartDate(string start)
    {
      Start = start;
    }
  }

  [System.Serializable]
  public class EndDate
  {
    public string End;
    public DateTime Date => DateTime.ParseExact(End, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    public EndDate(string end)
    {
      End = end;
    }
  }
}
