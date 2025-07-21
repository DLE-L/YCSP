using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Scripts.Calendar.Todos.Interaction
{
	public class TodoHome : MonoBehaviour, IOpenAble
	{
		public string UiName => "";

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			var todoUiManager = TodoUiManager.Instance;

			StartCoroutine(todoUiManager.TodoItemUpdate());
		}

		public void OnPointerDown(PointerEventData eventData)
		{

		}

		public void OnPointerUp(PointerEventData eventData)
		{

		}
	}
}