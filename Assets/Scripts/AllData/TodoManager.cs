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

    public void UpdateTodoManager(string csvData, string taskId)
    {
      string[] rows = csvData.Split('\n');
      int maxColumnCount = rows.Max(row => row.Split(',').Length);

      bool isExistKey = GroupLookup.ContainsKey(taskId);
      TodoGroup todoGroup = isExistKey ? GroupLookup[taskId] : new() { TaskId = taskId, TodoDatas = new() };

      var groupSpecificSetsLookup = new Dictionary<(DateTime, DateTime), List<TodoSet>>();
      if (isExistKey)
      {
        foreach (var todoData in todoGroup.TodoDatas)
        {
          var key = (todoData.StartDate.Date, todoData.EndDate.Date);
          groupSpecificSetsLookup[key] = todoData.TodoSets;
        }
      }
      
      todoGroup.TodoDatas.Clear();
      for (int row = 2; row < rows.Length; row++)
      {
        string[] rowNum = rows[row].Split(',');
        StartDate startDate = new(rowNum[0].Trim());
        EndDate endDate = new(rowNum[1].Trim());

        TodoData todoData = new()
        {
          StartDate = startDate,
          EndDate = endDate,
          TodoSets = new(),
        };

        if (rowNum.Length % 2 != 0)
        {
          Array.Resize(ref rowNum, rowNum.Length + 1);
          rowNum[^1] = null;
        }

        var key = (todoData.StartDate.Date, todoData.EndDate.Date);
        List<TodoSet> tempSets = new();
        int num = 0;
        for (int col = 2; col < maxColumnCount; col += 2)
        {
          if (string.IsNullOrEmpty(rowNum[col].Trim())) break;
          TodoSet todoSet = null;
          if (!groupSpecificSetsLookup.TryGetValue(key, out var sets) || sets.Count <= num)
          {
            // 키가 없거나, 기존 목록의 개수가 num보다 작을 경우 새로운 TodoSet 생성
            todoSet = new()
            {
              Todo = rowNum[col].Trim(),
              Note = rowNum[col + 1]?.Trim(),
            };
          }
          else
          {
            // 기존 목록에서 데이터를 가져와 수정
            todoSet = sets[num++];
            todoSet.Todo = rowNum[col].Trim();
            todoSet.Note = rowNum[col + 1]?.Trim();
          }
          tempSets.Add(todoSet);
        }
        todoData.TodoSets.AddRange(tempSets);
        todoGroup.TodoDatas.Add(todoData);
      }

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
