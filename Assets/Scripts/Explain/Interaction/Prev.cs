using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Explain.Interaction
{
    public class Prev : MonoBehaviour, IPointerClickHandler
    {
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            var explainUiManager = ExplainUiManager.Instance;
            explainUiManager.UpdateExplain(-1);
        }
    }
}