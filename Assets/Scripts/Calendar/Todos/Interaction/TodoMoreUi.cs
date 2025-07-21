using Scripts.AllData;
using Scripts.Calendar.Date;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace Scripts.Calendar.Todos.Interaction
{
  /// <summary>
  /// 할일 자세히 보기 ui
  /// </summary>
  public class TodoMoreUi : MonoBehaviour, IOpenAble
  {
    [SerializeField] private TextMeshProUGUI _startDate;
    [SerializeField] private TextMeshProUGUI _endDate;
    [SerializeField] private TextMeshProUGUI _todoContent;
    [SerializeField] private TextMeshProUGUI _todoNote;
    [SerializeField] private Graphic Checkmark;

    public string UiName => throw new System.NotImplementedException();

    public void UpdateMoreUi(TodoSet todoSet)
    {
      var dataManager = DataManager.Instance;
      TodoComplete todoComplete = dataManager.Complete.GetTodoComplete(todoSet.TodoId);
      TodoData todoData = dataManager.Todo.GetTodoData(todoSet.TodoId);
      _startDate.text = todoData.StartDate.Start;
      _endDate.text = todoData.EndDate.End;
      _todoContent.text = todoSet.Todo;
      _todoNote.text = todoSet.Note;
      Checkmark.enabled = (todoComplete.Complete != 0) ? true : false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      var todoUiManager = TodoUiManager.Instance;
      var calendarManager = CalendarUiManager.Instance;
      var dataManager = DataManager.Instance;
      Todo todo = todoUiManager.GetCurrentTodo();

      dataManager.Complete.SetTodoComplete(todo.todoSet.TodoId, Checkmark.enabled ? 1 : 0);

      todoUiManager.UpdateTodo(todo);
      //calendarManager.UpdateDay(todo.todoSet.TodoId);            
      todoUiManager.ClosePopup();
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
  }
}