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
    [SerializeField] private Transform _todoUi;
    [SerializeField] private TextMeshProUGUI _startDate;
    [SerializeField] private TextMeshProUGUI _endDate;

    /// <summary>
    /// 해당 날짜 할일 업데이트
    /// </summary>
    public void TodoUpdate(TodoData todoData)
    {
      if (todoData == null) return;
      
      _startDate.text = todoData.StartDate.Start;
      _endDate.text = todoData.EndDate.End;

      var dataManager = DataManager.Instance;
      var todoUiManager = TodoUiManager.Instance;
      var poolList = dataManager.Pool;
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
    }

    public IEnumerator ResetData()
    {
      int childCount = transform.childCount;
      for (int i = 1; i < childCount; i++)
      {
        var todo = transform.GetChild(1).GetComponent<Todo>();
        yield return todo.ResetData();
      }
      DataManager.Instance.Pool.Return<TodoItem>(gameObject);
    }
  }
}