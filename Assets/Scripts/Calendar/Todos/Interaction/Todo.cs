using System.Collections;
using Scripts.AllData;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Scripts.Calendar.Todos.Interaction
{
    /// <summary>
    /// 할일 클래스
    /// </summary>
    public class Todo : MonoBehaviour, IOpenAble
    {
        public TodoExpander todoItemExpander;
        public TextMeshProUGUI todoText;
        public TodoSet todoSet;
        public string UiName => "";

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            TodoUiManager todoUiManager = TodoUiManager.Instance;
            todoUiManager.TodoMoreUiUpdate(this);
        }

        public void TodoSet()
        {
            todoText.text = todoSet.Todo;
        }

        public IEnumerator ResetData()
        {
            todoSet = null;
            todoText.text = "이게 있으면 안되는디...";
            DataManager.Instance.PoolList.Return<Todo>(gameObject);
            yield return null;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            
        }
    }
}