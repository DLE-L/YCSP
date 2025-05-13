using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Explain.Interaction
{
    public class Close : MonoBehaviour, IPointerDownHandler
    {
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {            
            Debug.Log("설명서");
            ExplainUiManager.Instance.ClosePopup();
        }
    }
}