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
    void Start()
    {
      Instance = this;

      _todoPopup.SetActive(false);
      uiData = new()
      {
        {"TodoMoreUi", _todoMoreUi},
      };

      StartCoroutine(TodoItemUpdate());      
    }

    /// <summary>
    /// 해당 월 할일 아이템 데이터 업데이트
    /// </summary>
    public IEnumerator TodoItemUpdate(bool isAll = false)
    {
      var dataManager = DataManager.Instance;
      var todoManager = dataManager.Todo;
      var completeManager = dataManager.Complete;
      dataManager.CompareDate();
      DateTime currentDate = dataManager.currentDate;
      List<TodoData> listTodo = todoManager.GetBetweenDateTodo(currentDate) ?? new();

      listTodo = completeManager.GetCompleteList(listTodo, isAll);

      yield return ReturnGameObject();

      List<Transform> newTodoItems = new();
      foreach (var todoData in listTodo)
      {
        yield return SetTodo(todoData, newTodoItems);
      }
      for (int i = 0; i < newTodoItems.Count; i++)
      {
        newTodoItems[i].SetSiblingIndex(i);
      }
    }

    private IEnumerator SetTodo(TodoData todoData, List<Transform> newTodoItems)
    {
      yield return null;
      PoolManager poolList = DataManager.Instance.Pool;
      var todoItem = poolList.Get<TodoItem>(_todoList);
      todoItem.TodoUpdate(todoData);
      newTodoItems.Add(todoItem.transform);
    }

    public void UpdateTodo(Todo todo)
    {
      TodoComplete complete = DataManager.Instance.Complete.GetTodoComplete(todo.todoSet.TodoId);
      todo.gameObject.GetComponent<Image>().color = (complete.Complete != 0) ? new Color32(0x32, 0xCD, 0x32, 0xFF) : new Color32(0xff, 0x9a, 0xa2, 255);
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
      StartCoroutine(TodoItemUpdate());
      _todoPopup.SetActive(false);
      _currentTodo = null;
    }
  }
}