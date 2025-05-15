using UnityEngine;
using UnityEngine.EventSystems;
namespace Scripts.Tasks.Interaction
{
  public class LoadClipboard : MonoBehaviour, IPointerDownHandler
  {

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
      TaskUiManager.Instance.clipboardManager.PasteClipboardToInput();
    }
  }
}