using Scripts.AllData;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System;
using Scripts.Calendar.Todos.Interaction;

namespace Scripts.Calendar.Todos
{
    /// <summary>
    /// 해당 날짜 할일들을 가지고 있는 클래스
    /// </summary>
    public class TodoItem : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Transform _todoUi;
        [SerializeField] private TextMeshProUGUI _date;

        private const float BASE_HEIGHT = 100f; // 기본 높이
        private const float UNIT_HEIGHT = 100f; // Todo 하나의 높이
        void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        /// <summary>
        /// 해당 날짜 할일 업데이트
        /// </summary>
        public void TodoUpdate(TodoData todoData, DateTime time = default)
        {
            _date.text = time != default(DateTime) ? $"{time:yyyy-MM-dd}" : $"{todoData.Year}-{todoData.Month:D2}-{todoData.Day:D2}";
            if (todoData == null) return;

            var dataManager = DataManager.Instance;
            var todoUiManager = TodoUiManager.Instance;
            var poolList = dataManager.PoolList;
            int todoCount = todoData.TodoSets.Count;            

            List<Transform> newTodo = new();            
            for (int i = 0; i < todoCount; i++)
            {                
                var todo = poolList.Get<Todo>(transform);
                todo.todoSet = todoData.TodoSets[i];
                todo.TodoSet();
                todoUiManager.UpdateTodo(todo);
                newTodo.Add(todo.transform);
            }

            for (int i = 0; i < newTodo.Count; i++)
            {
                newTodo[i].SetAsLastSibling();
            }

            _todoUi.SetAsFirstSibling();

            //_rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, BASE_HEIGHT + todoCount * UNIT_HEIGHT);
        }

        public IEnumerator ResetData()
        {
            int childCount = transform.childCount;
            for (int i = 1; i < childCount; i++)
            {
                var todo = transform.GetChild(1).GetComponent<Todo>();
                yield return todo.ResetData();
            }
            DataManager.Instance.PoolList.Return<TodoItem>(gameObject);
        }
    }
}