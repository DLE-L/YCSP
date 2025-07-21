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
		public string UiName => "LoadingSlider";

		async void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			var dataManager = DataManager.Instance;
			var todoUiManager = TodoUiManager.Instance;
			var calendarUiManager = CalendarUiManager.Instance;

			todoUiManager.OpenPopup(UiName);
			int second = todoUiManager.StartLoading();
			dataManager.SetCanvasRaycast(false);
			await dataManager.Task.UpdateTaskData();
			await Task.Delay(second * 1000);
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