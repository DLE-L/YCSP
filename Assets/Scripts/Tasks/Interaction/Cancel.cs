using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Tasks.Interaction
{
  public class Cancel : MonoBehaviour, IPointerDownHandler
  {
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
      TaskUiManager.Instance.ClosePopup();
    }
  }
}