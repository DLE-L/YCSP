using System.Threading.Tasks;
using Scripts.AllData;
using Scripts.Calendar.Date;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Scripts.Calendar.Todos.Interaction
{
	/// <summary>
	/// 구글 시트 데이터 로드 상호작용
	/// </summary>
	public class TodoLoadGoogle : MonoBehaviour, IOpenAble
	{
		public string UiName => "";

		async void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			var dataManager = DataManager.Instance;
			var todoUiManager = TodoUiManager.Instance;
			var calendarUiManager = CalendarUiManager.Instance;

			dataManager.SetCanvasRaycast(false);
			await dataManager.Task.UpdateTaskData();
			todoUiManager.ClosePopup();
			calendarUiManager.ShowCalendarUi();
			dataManager.SetCanvasRaycast(true);
		}

		public void OnPointerDown(PointerEventData eventData)
		{

		}

		public void OnPointerUp(PointerEventData eventData)
		{

		}
	}
}