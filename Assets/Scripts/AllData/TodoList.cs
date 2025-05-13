using Utils;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Scripts.AllData
{
    [System.Serializable]
    public class TodoList
    {
        [NonSerialized] public TaskData TaskData = new();
        [NonSerialized] private Dictionary<string, (int, int, int)> TodoSetDateIndex = new();
        [NonSerialized] private Dictionary<string, TaskTodoGroup> TaskLookup = new();
        [NonSerialized] private Dictionary<string, Dictionary<(int, int), List<TodoData>>> MonthLookup = new(); // 해당 월 todo data 반환
        [NonSerialized] private Dictionary<string, Dictionary<(int, int, int), List<TodoData>>> DayLookup = new(); // 해당 일 todo data 반환
        [NonSerialized] private Dictionary<string, TodoSet> TodoSetLookup = new(); // todoid의 complete 검색
        [NonSerialized] private Dictionary<string, TodoCache> CacheLookup = new(); // 
        [SerializeField] private List<CacheList> CacheList = new();
        [SerializeField] private List<TaskTodoGroup> TodoGroup = new();

        private void Save(string fileName = "Todos")
        {
            JsonManager.SaveJson<TodoList>(fileName, this);
        }

        public TodoList Load(string fileName = "Todos")
        {
            TodoList tempList = JsonManager.LoadJson<TodoList>(fileName);
            if (tempList != null)
            {
                TodoGroup = tempList.TodoGroup;
                CacheList = tempList.CacheList;

                UpdateLookupTable();
            }
            return tempList ?? new TodoList();
        }

        public List<TodoData> GetTodoMonthDatas(DateTime date)
        {
            var key = (date.Year, date.Month);
            if (MonthLookup.TryGetValue(TaskData.TaskUrl, out var monthData) && monthData.TryGetValue(key, out var list))
            {
                return list;
            }
            return new List<TodoData>();
        }

        public TodoData GetTodoDayData(DateTime date)
        {
            var key = (date.Year, date.Month, date.Day);
            if (DayLookup.TryGetValue(TaskData.TaskUrl, out var data) && data.TryGetValue(key, out var list))
            {
                return list.Find(todo => todo.Year == key.Year && todo.Month == key.Month && todo.Day == key.Day);
            }
            return new TodoData();
        }

        /// <summary>
        /// 캐싱 데이터 유무 체크 & 구글 시트 월 데이터 로드
        /// </summary>
        public async Task<string> UpdateFromGoogleSheet()
        {
            TodoGroup.Clear();
            var dataManager = DataManager.Instance;
            var today = dataManager.Today;
            foreach (var data in TaskData.Urls)
            {
                // 현재 달보다 이전이면 건너뜀
                if(data.Month < today.Month) continue;

                string csvData = await GoogleSheetManager.LoadCSVData(data.Url);     // 해당 월 시트 데이터 로드

                if (string.IsNullOrEmpty(csvData))
                {
                    return "데이터를 가져오지 못했습니다";
                }
                SaveTodoData(csvData);

                await Task.Yield();
            }
            UpdateLookupTable();
            Save();
            return null;
        }

        private void SaveTodoData(string csvData)
        {
            string[] rows = csvData.Split('\n');
            TodoData tempTodo = null;

            var taskGroup = TodoGroup.Find(task => task.TaskUrl == TaskData.TaskUrl);

            if (taskGroup == null)
            {
                taskGroup = new TaskTodoGroup() { TaskUrl = TaskData.TaskUrl };
                
                TodoGroup.Add(taskGroup);
            }

            for (int i = 1; i < rows.Length; i++)
            {
                string[] columns = rows[i].Split(',');
                string todoText = columns[1].Trim();

                if (columns.Length < 2) continue;
                if (string.IsNullOrEmpty(todoText)) continue;

                string note = columns.Length > 2 ? columns[2].Trim() : string.Empty;
                TodoSet todoSet = new TodoSet() { Todo = todoText, Note = note };
                // 날짜 열이 비어있으면 전날의 할 일 추가
                if (string.IsNullOrEmpty(columns[0]))
                {
                    tempTodo.TodoSets.Add(todoSet);
                }
                else
                {
                    string[] sheetDate = columns[0].Split('-');
                    tempTodo = new TodoData()
                    {
                        Year = int.Parse(sheetDate[0].Trim()),
                        Month = int.Parse(sheetDate[1].Trim()),
                        Day = int.Parse(sheetDate[2].Trim()),
                        TodoSets = new List<TodoSet> { todoSet },
                    };
                    taskGroup.TodoDatas.Add(tempTodo);
                }
            }
        }

        public DateTime GetTodoDate(TodoSet todoSet)
        {
            var key = TodoSetDateIndex[todoSet.TodoId];
            DateTime date = new(key.Item1, key.Item2, key.Item3);
            return date;
        }

        public void UpdateTodoComplete(TodoSet todoSet)
        {
            var setId = todoSet.TodoId;
            if (!TodoSetLookup.TryGetValue(setId, out var set)) return;
            var completeStatus = todoSet.Complete;

            if (!CacheLookup.TryGetValue(setId, out var cache))
            {
                cache = new TodoCache() { TodoId = setId, Complete = completeStatus };
                CacheLookup[setId] = cache;

                var cacheList = CacheList.Find(list => list.TaskUrl == TaskData.TaskUrl);

                if (cacheList == null)
                {
                    cacheList = new CacheList() { TaskUrl = TaskData.TaskUrl };
                    CacheList.Add(cacheList);
                    Save();
                }
                cacheList.TodoCaches.Add(cache);
            }

            if (cache.Complete != completeStatus || set.Complete != completeStatus)
            {
                cache.Complete = completeStatus;
                set.Complete = completeStatus;
                Save();
            }
        }

        private void DictionaryToList()
        {
            TodoGroup.Clear();
            foreach (var taskUrl in DayLookup)
            {
                var group = new TaskTodoGroup { TaskUrl = taskUrl.Key, TodoDatas = new() };
                foreach (var todoData in taskUrl.Value)
                {
                    group.TodoDatas.AddRange(todoData.Value);
                }
                TodoGroup.Add(group);
            }
        }

        private void UpdateLookupTable()
        {
            TaskLookup.Clear();
            MonthLookup.Clear();
            DayLookup.Clear();
            CacheLookup.Clear();
            TodoSetLookup.Clear();

            foreach (var group in TodoGroup)
            {
                if (!TaskLookup.ContainsKey(group.TaskUrl))
                {
                    TaskLookup[group.TaskUrl] = group;
                }
                if (!DayLookup.ContainsKey(group.TaskUrl)) DayLookup[group.TaskUrl] = new();
                if (!MonthLookup.ContainsKey(group.TaskUrl)) MonthLookup[group.TaskUrl] = new();
                foreach (var todo in group.TodoDatas)
                {
                    var key = (todo.Year, todo.Month, todo.Day);
                    var monthKey = (todo.Year, todo.Month);
                    if (!DayLookup[group.TaskUrl].ContainsKey(key)) DayLookup[group.TaskUrl][key] = new();
                    if (!MonthLookup[group.TaskUrl].ContainsKey(monthKey)) MonthLookup[group.TaskUrl][monthKey] = new();
                }
            }

            foreach (var group in TodoGroup)
            {
                foreach (var todo in group.TodoDatas)
                {
                    var key = (todo.Year, todo.Month, todo.Day);
                    var monthKey = (todo.Year, todo.Month);
                    DayLookup[group.TaskUrl][key].Add(todo);
                    MonthLookup[group.TaskUrl][monthKey].Add(todo);

                    foreach (var set in todo.TodoSets)
                    {
                        if (!TodoSetLookup.ContainsKey(set.TodoId))
                        {
                            TodoSetLookup[set.TodoId] = set;
                            TodoSetDateIndex[set.TodoId] = (todo.Year, todo.Month, todo.Day);
                        }
                    }
                }
            }

            foreach (var caches in CacheList)
            {
                foreach (var cache in caches.TodoCaches)
                {
                    if (TodoSetLookup.TryGetValue(cache.TodoId, out var todoSet))
                    {
                        todoSet.Complete = cache.Complete;
                    }
                    CacheLookup[cache.TodoId] = cache;
                }
            }
        }

        public void RemoveTodoData(TaskData task)
        {
            if (TaskLookup.TryGetValue(task.TaskUrl, out var data))
            {
                int index = TodoGroup.FindIndex(group => group.TaskUrl == data.TaskUrl);
                TodoGroup.RemoveAt(index);
                Save();
            }
        }
    }

    [System.Serializable]
    public class TaskTodoGroup
    {
        public string TaskUrl;
        public List<TodoData> TodoDatas = new();
    }


    [System.Serializable]
    public class TodoData
    {
        public int Year;
        public int Month;
        public int Day;
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
        public string TaskUrl;
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
    1. 일정 데이터 구조
        public class TaskData
        {
            public string TaskUrl;
            public string TitleName;
            public List<string> Urls = new List<string>();
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
