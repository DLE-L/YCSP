using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Scripts.Explain.Interaction
{
    public class ExplainIcon : MonoBehaviour, IOpenAble
    {
        public string UiName => "ExplainSelect";

        public void OnPointerClick(PointerEventData eventData)
        {
            ExplainUiManager.Instance.OpenPopup(UiName);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            
        }

        public void OnPointerUp(PointerEventData eventData)
        {

        }
    }
}