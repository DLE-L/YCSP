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
        [NonSerialized] private Dictionary<(StartDate, EndDate), List<TodoSet>> TodoLookup = new();
        [SerializeField] private List<TodoData> Todos = new();
        [SerializeField] public string TodoManagerId = Guid.NewGuid().ToString();

        public void InitTodoManager(string csvData)
        {
            string[] rows = csvData.Split('\n');

            for (int row = 2; row < rows.Length; row++)
            {
                string[] rowN = rows[row].Split(',');
                string[] start = rowN[0].Split('-');
                string[] end = rowN[1].Split('-');
                StartDate startDate;
                {
                    int year = int.Parse(start[0]);
                    int month = int.Parse(start[1]);
                    int day = int.Parse(start[2]);
                    startDate = new(year, month, day);
                }
                EndDate endDate;
                {
                    int year = int.Parse(end[0]);
                    int month = int.Parse(end[1]);
                    int day = int.Parse(end[2]);
                    endDate = new(year, month, day);
                }               

                TodoData todoData = new()
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    TodoSets = new(),
                };
                TodoSet todoSet;
                for (int col = 2; col < rows[row].Length; col++)
                {
                    string[] todonote = rowN[col].Split('/');
                    todoSet = new()
                    {
                        Todo = todonote[0].Trim(),
                        Note = todonote[1].Trim(),
                    };
                    todoData.TodoSets.Add(todoSet);
                }                
                Todos.Add(todoData);
                TodoLookup[(startDate, endDate)] = todoData.TodoSets;
            }
            Save();
        }


        /// <summary>
        /// 실행시 딕셔너리 테이블 세팅
        /// </summary>
        public void SetLookupTable()
        {
            TodoLookup.Clear();
            foreach (var todoData in Todos)
            {
                var key = (todoData.StartDate, todoData.EndDate);
                if (!TodoLookup.TryGetValue(key, out var todoSets))
                {
                    todoSets = new();
                    TodoLookup[key] = todoSets;
                }
                todoSets.AddRange(todoData.TodoSets);
            }
        }

        public List<TodoSet> GetBetweenDateTodo(DateTime nowDate)
        {
            return TodoLookup.Where(todo => todo.Key.Item1.startDate <= nowDate && todo.Key.Item2.endDate >= nowDate)
                            .SelectMany(todo=> todo.Value)
                            .ToList();
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
                SetLookupTable();
            }
            return tempTodo ?? new TodoManager();
        }

        public void UpdateTodoComplete(TodoSet todoSet)
        {
 
        }
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
