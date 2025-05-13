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
        // https://docs.google.com/spreadsheets/d/1XCO4tWxuM0OO-ih9mptPGTzUWaDn1GaB-oygUSJLL84/edit?gid=0#gid=0
        public static DataManager Instance;

        public ItemInfo itemInfo;
        public List<Day> days = new List<Day>();

        public TaskList TaskList { get; private set; }
        public TodoList TodoList { get; private set; }
        public CalendarList CalendarList { get; private set; }
        public PoolList PoolList { get; private set; }
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
            TaskList = new TaskList();
            TodoList = new TodoList();
            CalendarList = new CalendarList();
            PoolList = new PoolList();
            PoolList.typeContainer = new Dictionary<Type, GameObject>()
            {
                {typeof(TaskItem), itemInfo.taskItem.gameObject},
                {typeof(TodoItem), itemInfo.todoItem.gameObject},
                {typeof(Todo), itemInfo.todo.gameObject},
                {typeof(Day), itemInfo.day.gameObject}
            };

            TaskList.Load();
            TodoList.Load();
            CalendarList.LoadCalendarData();

            currentDate = Today = DateTime.Now;
        }

        public void ConnectTaskAndTodo(TaskData taskData)
        {
            TodoList.TaskData = taskData;
        }

        public void CompareDate()
        {
            currentDate = (currentDate.Year == Today.Year && currentDate.Month == Today.Month) ? Today : new DateTime(currentDate.Year, currentDate.Month, 1);
        }

        public void SetCanvasRaycast(bool acitve)
        {
            var calendar = CalendarUiManager.Instance;
            var task = TaskUiManager.Instance;
            var explain =ExplainUiManager.Instance;

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
}
