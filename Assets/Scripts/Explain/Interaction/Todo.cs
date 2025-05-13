using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Scripts.Explain.Interaction
{
    public class Todo : MonoBehaviour, IOpenAble
    {
        public string UiName => "ExplainUi";

        public void OnPointerClick(PointerEventData eventData)
        {
            
        }

        public void OnPointerUp(PointerEventData eventData)
        {

        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            var explainUiManager = ExplainUiManager.Instance;
            explainUiManager.ClosePopup();
            explainUiManager.OpenPopup(UiName, gameObject.name);
        }
    }
}