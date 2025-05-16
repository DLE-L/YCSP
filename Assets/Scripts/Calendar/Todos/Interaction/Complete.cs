using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace Scripts.Calendar.Todos.Interaction
{
    public class Complete : MonoBehaviour, IOpenAble
    {
        public Graphic checkmark;

        public string UiName => "";

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            checkmark.enabled = !checkmark.enabled;
        }

        public void OnPointerDown(PointerEventData eventData)
        {

        }

        public void OnPointerUp(PointerEventData eventData)
        {

        }
    }
}