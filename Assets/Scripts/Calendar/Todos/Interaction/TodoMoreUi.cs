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
        [SerializeField] private TextMeshProUGUI todoContent;
        [SerializeField] private TextMeshProUGUI todoNote;
        [SerializeField] private Graphic Checkmark;

        public string UiName => throw new System.NotImplementedException();

        public void UpdateMoreUi(TodoSet todoSet)
        {
            todoContent.text = todoSet.Todo;
            todoNote.text = todoSet.Note;
            Checkmark.enabled = (todoSet.Complete != 0) ? true : false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var todoUiManager = TodoUiManager.Instance;
            var dataManager = DataManager.Instance;
            var todo = todoUiManager.GetCurrentTodo();
            todo.todoSet.Complete = Checkmark.enabled ? 1 : 0;
            dataManager.TodoList.UpdateTodoComplete(todo.todoSet);
            
            CalendarUiManager.Instance.ShowDayOnly(todo);
            todoUiManager.UpdateTodo(todo);
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