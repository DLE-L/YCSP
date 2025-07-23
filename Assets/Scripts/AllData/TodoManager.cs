using Utils;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Scripts.AllData
{
  [System.Serializable]
  public class TodoManager
  {
    [NonSerialized] public Dictionary<string, int> TodoIdLookup = new();
    [NonSerialized] private Dictionary<(DateTime, DateTime), List<TodoSet>> SetsLookup = new();
    [NonSerialized] public Dictionary<string, TodoGroup> GroupLookup = new();
    [NonSerialized] private Dictionary<string, TodoData> DataLookup = new();
    [NonSerialized] private Dictionary<string, TodoSet> SetLookup = new();
    [SerializeField] public List<TodoGroup> Todos = new();

    /// <summary>
    /// 할 일 목록 업데이트
    /// </summary>
    public void UpdateTodoManager(string csvData, string taskId)
    {
      string[] rows = csvData.Split('\n');
      bool isExistKey = GroupLookup.ContainsKey(taskId);
      TodoGroup todoGroup = isExistKey ? GroupLookup[taskId] : new() { TaskId = taskId, TodoDatas = new() };

      // TodoSet조회 딕셔너리 생성
      var existingTodoSets = new Dictionary<(DateTime, DateTime, string), TodoSet>();
      if (isExistKey)
      {
        foreach (var todoData in todoGroup.TodoDatas)
        {
          foreach (var todoSet in todoData.TodoSets)
          {
            var key = (todoData.StartDate.Date, todoData.EndDate.Date, todoSet.Todo);
            existingTodoSets.TryAdd(key, todoSet);
          }
        }
      }
      
      // 새로운 데이터 구조 생성, 중복 필터링
      var newTodoDatas = new List<TodoData>();
      var processedTodos = new HashSet<(DateTime, DateTime, string)>(); // CSV 내 중복 체크용

      for (int row = 2; row < rows.Length; row++)
      {
        string[] rowNum = rows[row].Split(',');
        
        // 시트에서 불러오는 열 개수는 4개(시작일, 종료일, 할일, 노트)
        // 노트는 생략 가능
        if (rowNum.Length < 3 || string.IsNullOrEmpty(rowNum[0].Trim())) continue;

        StartDate startDate = new(rowNum[0].Trim());
        EndDate endDate = new(rowNum[1].Trim());
        string todoText = rowNum[2].Trim();
        string noteText = rowNum.Length > 3 ? rowNum[3]?.Trim() : null;

        if (string.IsNullOrEmpty(todoText)) continue;

        // CSV 데이터 내에서 중복된 내용(시작일, 종료일, 할일 내용이 동일)은 생략
        var todoKey = (startDate.Date, endDate.Date, todoText);
        if (processedTodos.Contains(todoKey))
        {
          continue;
        }
        processedTodos.Add(todoKey);

        // 각 행마다 새로운 TodoData를 생성
        var todoData = new TodoData
        {
          StartDate = startDate,
          EndDate = endDate,
          TodoSets = new List<TodoSet>()
        };

        // 기존에 있던 할일이면 ID유지, 새로운 할일이면 새로 생성
        TodoSet todoSet;
        if (existingTodoSets.TryGetValue(todoKey, out var existingSet))
        {          
          todoSet = existingSet;
          todoSet.Note = noteText;
        }
        else
        {
          todoSet = new TodoSet
          {
            Todo = todoText,
            Note = noteText
          };
        }
        todoData.TodoSets.Add(todoSet);
        newTodoDatas.Add(todoData);
      }

      // 기존의 TodoDatas를 새로 생성된 데이터로 교체합니다.
      todoGroup.TodoDatas = newTodoDatas;

      if (!isExistKey)
      {
        Todos.Add(todoGroup);
        GroupLookup[taskId] = todoGroup;
        TodoIdLookup[taskId] = Todos.Count - 1;

        var dataManager = DataManager.Instance;
        TodoCompleteManager todoCompleteManager = dataManager.Complete;
        todoCompleteManager.UpdateTodoCompleteManager(todoGroup);
      }

      SetLookupTable();
      Save();
    }

    public List<TodoData> GetBetweenDateTodo(DateTime nowDate)
    {
      var dataManager = DataManager.Instance;

      if (dataManager.Task.CurrentTaskId == null) return null;

      TodoGroup todoGroup = GroupLookup[dataManager.Task.CurrentTaskId];

      var firstDayOfMonth = new DateTime(nowDate.Year, nowDate.Month, 1);
      var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

      return todoGroup.TodoDatas.FindAll(todo =>
          todo.StartDate.Date.Date <= lastDayOfMonth && todo.EndDate.Date.Date >= firstDayOfMonth
      ).OrderBy(todo => todo.EndDate.Date).ToList();
    }

    public TodoData GetTodoData(string todoId)
    {
      return DataLookup[todoId];
    }

    public TodoSet GetTodoSet(string todoId)
    {
      return SetLookup[todoId];
    }

    /// <summary>
    /// 실행시 딕셔너리 테이블 세팅
    /// </summary>
    public void SetLookupTable()
    {
      SetsLookup.Clear();
      DataLookup.Clear();
      SetLookup.Clear();
      TodoIdLookup.Clear();
      int count = 0;
      foreach (var todoGroup in Todos)
      {
        string taskId = todoGroup.TaskId;
        GroupLookup[taskId] = todoGroup;
        TodoIdLookup[taskId] = count++;
        foreach (var todoData in todoGroup.TodoDatas)
        {
          var key = (todoData.StartDate.Date, todoData.EndDate.Date);
          SetsLookup[key] = todoData.TodoSets;
          foreach (var todoSet in todoData.TodoSets)
          {
            DataLookup[todoSet.TodoId] = todoData;
            SetLookup[todoSet.TodoId] = todoSet;
          }
        }
      }
    }

    public void RemoveTodo(string taskId)
    {
      if (!GroupLookup.ContainsKey(taskId)) return;
      
      var groupToRemove = GroupLookup[taskId];
      Todos.Remove(groupToRemove);
      SetLookupTable();
      Save();
    }
    
    public void Save(string fileName = "Todos")
    {
      JsonManager.SaveJson<TodoManager>(fileName, this);
    }

    public TodoManager Load(string fileName = "Todos")
    {
      var tempList = JsonManager.LoadJson<TodoManager>(fileName);
      if (tempList != null)
      {
        Todos = tempList.Todos;
        SetLookupTable();
      }
      return tempList ?? new TodoManager();
    }
  }

  [System.Serializable]
  public class TodoGroup
  {
    public string TaskId;
    public List<TodoData> TodoDatas = new();
  }

  [System.Serializable]
  public class TodoData
  {
    public StartDate StartDate;
    public EndDate EndDate;
    public List<TodoSet> TodoSets = new();
  }

  [System.Serializable]
  public class TodoSet
  {
    public string TodoId = Guid.NewGuid().ToString();
    public string Todo;
    public string Note;
  }
}
/*
    혹시 모를 비교
    public override bool Equals(object obj)
    {
        if (obj is EndDate other)
        {
            return Year == other.Year && Month == other.Month && Day == other.Day;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Year, Month, Day);
    }

    public static bool operator ==(EndDate left, EndDate right)
    {
        if (left is null || right is null) return Equals(left, right);
        return left.Equals(right);
    }

    public static bool operator !=(EndDate left, EndDate right)
    {
        return !(left == right);
    }
*/

/*
    1. 일정 데이터 구조
        public class TaskData
        {
            public string TaskUrl;
            public string TitleName;
        }

    2. 캘린더 데이터 구조
        public class YearData
        {
            public int year;
            public List<MonthData> months = new List<MonthData>();
        }

        public class MonthData
        {
            public int month;
            public List<int> days = new List<int>();
        }

        public class CalendarList
        {
            private List<YearData> data = new List<YearData>();
        }

    3. 구글 시트구조
        Date    , Todo       , Note
        string  , List<Todo> , string
*/
