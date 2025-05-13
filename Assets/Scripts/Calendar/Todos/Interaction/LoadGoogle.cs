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
    public class LoadGoogle : MonoBehaviour, IOpenAble
    {
        public string UiName => "LoadingSlider";

        void Update()
        {
            // 개인 일정 추가 하면?
            // if (TimeManager.HoldingOneSeconds(ref _isHolding, ref _holdTime))
            // {
            //     TodoUiManager.Instance.OpenPopup(gameObject);
            // }
        }

        async void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            var todoUiManager = TodoUiManager.Instance;
            var calendarUiManager = CalendarUiManager.Instance;
            var dataManager = DataManager.Instance;            
            todoUiManager.OpenPopup(UiName);    
            var second = todoUiManager.StartLoading();        
            dataManager.SetCanvasRaycast(false);            
            await DataManager.Instance.TodoList.UpdateFromGoogleSheet();            
            await Task.Delay(second * 1000);            
            todoUiManager.ClosePopup();
            calendarUiManager.ShowCalendarUi();
            StartCoroutine(todoUiManager.TodoItemUpdate());
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