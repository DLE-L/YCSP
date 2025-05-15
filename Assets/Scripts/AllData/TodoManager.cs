using Utils;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Scripts.AllData
{
  [System.Serializable]
  public class TodoManager
  {
    [NonSerialized] public string CurrentTaskId;
    [NonSerialized] private Dictionary<string, TodoGroup> GroupLookup = new();
    [NonSerialized] private Dictionary<string, TodoData> DataLookup = new();
    [NonSerialized] private Dictionary<(StartDate, EndDate), List<TodoSet>> SetsLookup = new();
    [NonSerialized] private Dictionary<string, TodoSet> SetLookup = new();
    [NonSerialized] private Dictionary<string, TodoCache> CacheLookup = new();
    [SerializeField] private List<CacheList> Caches = new();
    [SerializeField] private List<TodoGroup> Todos = new();

    public void InitTodoManager(string csvData, string taskId)
    {
      string[] rows = csvData.Split('\n');
      int maxColumnCount = rows.Max(row => row.Split(',').Length);

      TodoGroup todoGroup = new()
      {
        TaskId = taskId,
        TodoDatas = new(),
      };
      for (int row = 2; row < rows.Length; row++)
      {
        string[] rowNum = rows[row].Split(',');
        string[] start = rowNum[0].Split('-');
        string[] end = rowNum[1].Split('-');

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
          DataLookup[todoSet.TodoId] = todoData;
        }
        todoGroup.TodoDatas.Add(todoData);
        SetsLookup[(startDate, endDate)] = todoData.TodoSets;
      }
      Todos.Add(todoGroup);
      if (!GroupLookup.ContainsKey(taskId))
      {
        GroupLookup[taskId] = todoGroup;
      }

      Save();
    }

    /// <summary>
    /// 실행시 딕셔너리 테이블 세팅
    /// </summary>
    public void SetLookupTable()
    {
      SetsLookup.Clear();
      DataLookup.Clear();
      GroupLookup.Clear();
      foreach (var todoGroup in Todos)
      {
        string taskId = todoGroup.TaskId;
        if (!GroupLookup.ContainsKey(taskId))
        {
          GroupLookup[taskId] = todoGroup;
        }

        foreach (var todoData in todoGroup.TodoDatas)
        {
          var key = (todoData.StartDate, todoData.EndDate);
          if (!SetsLookup.TryGetValue(key, out var todoSets))
          {
            todoSets = new();
            SetsLookup[key] = todoSets;
          }
          todoSets.AddRange(todoData.TodoSets);

          foreach (var todoSet in todoData.TodoSets)
          {
            DataLookup[todoSet.TodoId] = todoData;
          }
        }
      }
    }

    public List<TodoData> GetBetweenDateTodo(DateTime nowDate)
    {
      TodoGroup todoGroup = GroupLookup[CurrentTaskId];
      return todoGroup.TodoDatas.FindAll(todo => todo.StartDate.Date <= nowDate && todo.EndDate.Date >= nowDate).ToList();
    }

    public TodoData GetTodoData(string todoId)
    {
      return DataLookup[todoId];
    }

    public void UpdateTodoComplete(TodoSet todoSet)
    {
      string setId = todoSet.TodoId;
      if (!SetLookup.TryGetValue(setId, out var set)) return;
      int complete = todoSet.Complete;

      if (!CacheLookup.TryGetValue(setId, out var cache))
      {
        cache = new TodoCache() { TodoId = setId, Complete = complete };
        CacheLookup[setId] = cache;

        CacheList cacheList = Caches.Find(list => list.TaskId == CurrentTaskId);

        if (cacheList == null)
        {
          cacheList = new CacheList() { TaskId = CurrentTaskId };
          Caches.Add(cacheList);
          Save();
        }
        cacheList.TodoCaches.Add(cache);
      }

      if (cache.Complete != complete || set.Complete != complete)
      {
        cache.Complete = complete;
        set.Complete = complete;
        Save();
      }
    }
    
    private void Save(string fileName = "Todos")
    {
      JsonManager.SaveJson<TodoManager>(fileName, this);
    }

    public TodoManager Load(string fileName = "Todos")
    {
      TodoManager tempTodo = JsonManager.LoadJson<TodoManager>(fileName);
      if (tempTodo != null)
      {
        Todos = tempTodo.Todos;
        Caches = tempTodo.Caches;
        SetLookupTable();
      }
      return tempTodo ?? new TodoManager();
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
    public int Complete;
  }

  [System.Serializable]
  public class CacheList
  {
    public string TaskId;
    public List<TodoCache> TodoCaches = new();
  }

  [System.Serializable]
  public class TodoCache
  {
    public string TodoId;
    public int Complete;
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
