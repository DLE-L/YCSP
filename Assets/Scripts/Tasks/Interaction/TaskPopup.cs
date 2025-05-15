using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Scripts.Tasks.Interaction
{
  public class TaskPopup : MonoBehaviour, IOpenAble
  {
    public string UiName => "SheetUrl";

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
      TaskUiManager.Instance.OpenPopup(UiName);
    }
  }
}