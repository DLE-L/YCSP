using Utils;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics.Geometry;

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

        for (int col = 2; col < maxColumnCount; col += 2)
        {
          if (string.IsNullOrEmpty(rowNum[col].Trim())) break;
          TodoSet todoSet = new()
          {
            Todo = rowNum[col].Trim(),
            Note = rowNum[col + 1]?.Trim(),
          };
          todoData.TodoSets.Add(todoSet);
          SetLookup[todoSet.TodoId] = todoSet;
          DataLookup[todoSet.TodoId] = todoData;
        }
        todoGroup.TodoDatas.Add(todoData);
        SetsLookup[(startDate.Date, endDate.Date)] = todoData.TodoSets;
      }

      if (!isExistKey)
      {
        Todos.Add(todoGroup);
        GroupLookup[taskId] = todoGroup;
        TodoIdLookup[taskId] = Todos.Count - 1;

        var dataManager = DataManager.Instance;
        TodoCompleteManager todoCompleteManager = dataManager.Complete;
        todoCompleteManager.InitTodoCompleteManager(todoGroup);
      }

      Save();
    }

    /// <summary>
    /// 실행시 딕셔너리 테이블 세팅
    /// </summary>
    private void SetLookupTable()
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

    public List<TodoData> GetBetweenDateTodo(DateTime nowDate)
    {
      var dataManager = DataManager.Instance;
      TodoGroup todoGroup = GroupLookup[dataManager.Task.CurrentTaskId];
      return todoGroup.TodoDatas.FindAll(todo => todo.StartDate.Date <= nowDate.Date && nowDate.Date <= todo.EndDate.Date).ToList();
    }

    public TodoData GetTodoData(string todoId)
    {
      return DataLookup[todoId];
    }

    public TodoSet GetTodoSet(string todoId)
    {
      return SetLookup[todoId];
    }

    public void RemoveTodoData(string taskId)
    {
      int index = TodoIdLookup[taskId];
      Todos.RemoveAt(index);

      SetLookupTable();
      Save();
    }
    
		private void Save(string fileName = "Todos")
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
