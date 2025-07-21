using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Scripts.Calendar.Todos.Interaction
{
	public class TodoAll : MonoBehaviour, IOpenAble
	{
		public string UiName => "";

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			var todoUiManager = TodoUiManager.Instance;

			StartCoroutine(todoUiManager.TodoItemUpdate(true));
		}

		public void OnPointerDown(PointerEventData eventData)
		{

		}

		public void OnPointerUp(PointerEventData eventData)
		{

		}
	}
}