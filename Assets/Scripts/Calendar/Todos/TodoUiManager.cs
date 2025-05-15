using System.Collections.Generic;
using System.Collections;
using Scripts.AllData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Scripts.Calendar.Todos.Interaction;
using Utils;

namespace Scripts.Calendar.Todos
{
    /// <summary>
    /// 할일 ui관련 매니저
    /// </summary>
    public class TodoUiManager : UiManager
    {
        public static TodoUiManager Instance;
        [SerializeField] private RectTransform _todoList;
        [SerializeField] private GameObject _todoPopup;
        [SerializeField] private GameObject _todoMoreUi;
        [SerializeField] private Slider _loadingSlider;
        [SerializeField] private TextMeshProUGUI _loadingText;

        public override Dictionary<string, GameObject> uiData { get; set; } = new();
        public override Stack<GameObject> OpenUis { get; set; } = new();

        private Todo _currentTodo;

        private int _loadingSecond;

        void Start()
        {
            Instance = this;

            _todoPopup.SetActive(false);
            uiData = new()
            {
                {"LoadingSlider", _loadingSlider.gameObject},
                {"TodoMoreUi", _todoMoreUi},
            };
        }

        /// <summary>
        /// 해당 월 할일 아이템 데이터 업데이트
        /// </summary>
        /// <returns></returns>
        public IEnumerator TodoItemUpdate()
        {            
            DataManager dataManager = DataManager.Instance;
            DateTime currentDate = dataManager.currentDate;
            List<TodoData> listTodoData = dataManager.Todo.GetBetweenDateTodo(currentDate);
            
            PoolManager poolList = dataManager.Pool;
            int listCount = listTodoData.Count;

            yield return ReturnGameObject();

            if (listCount < 1)
            {
                yield break;
            }
            List<Transform> newTodoItems = new();
            foreach (var todoData in listTodoData)
            {
                var todoItem = poolList.Get<TodoItem>(_todoList);
                todoItem.TodoUpdate(todoData);
                newTodoItems.Add(todoItem.transform);
            }
            for (int i = 0; i < newTodoItems.Count; i++)
            {
                newTodoItems[i].SetSiblingIndex(i);
            }
        }

        public void UpdateTodo(Todo todo)
        {
            todo.gameObject.GetComponent<Image>().color = (todo.todoSet.Complete != 0) ? new Color32(0x32, 0xCD, 0x32, 0xFF) : new Color32(0xff, 0x9a, 0xa2, 255);
            // new Color32(0x32, 0xCD, 0x32, 0xFF);
        }

        /// <summary>
        /// 풀링 ui관련 함수
        /// </summary>
        private IEnumerator ReturnGameObject()
        {
            int childCount = _todoList.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var todoItem = _todoList.GetChild(0).GetComponent<TodoItem>();
                yield return todoItem.ResetData();
            }
            yield return null;
        }

        /// <summary>
        /// 구글 시트 데이터 가져올때 로딩 바
        /// </summary>
        public int StartLoading()
        {
            _loadingSlider.value = 0f;
            StartCoroutine(Loading());
            return _loadingSecond;
        }

        public IEnumerator Loading()
        {
            var dataManager = DataManager.Instance;
            var count = 12 - dataManager.Today.Month + 1;
            _loadingSecond = count;

            float delaySeconds = count;
            float time = 0;
            while (time < delaySeconds)
            {
                time += Time.deltaTime;
                float progress = time / delaySeconds;
                _loadingSlider.value = progress;
                _loadingText.text = $"{progress * 100f:F0} %";
                yield return null;
            }
            _loadingSlider.value = 1f;
        }

        /// <summary>
        /// 할 일 세부사항 ui
        /// </summary>
        public void TodoMoreUiUpdate(Todo todo)
        {
            var expander = todo.todoItemExpander;
            var moreUi = _todoMoreUi.GetComponent<TodoMoreUi>();

            expander.todoMoreUI = _todoMoreUi.GetComponent<RectTransform>();
            expander.canvas = transform.root.GetComponent<Canvas>();
            expander.popup = _todoPopup;
            expander.StartExpand(todo.gameObject.GetComponent<RectTransform>());

            moreUi.UpdateMoreUi(todo.todoSet);
            _currentTodo = todo;
        }

        public Todo GetCurrentTodo()
        {
            return _currentTodo;
        }

        public override void OpenPopup(string uiName, string selfName = null)
        {
            _todoPopup.SetActive(true);
            base.OpenPopup(uiName);
            foreach (var item in uiData)
            {

                if (item.Key != uiName)
                {
                    item.Value.SetActive(false);
                }
            }
        }

        public override void ClosePopup()
        {
            base.ClosePopup();
            _todoPopup.SetActive(false);
            _currentTodo = null;
        }
    }
}