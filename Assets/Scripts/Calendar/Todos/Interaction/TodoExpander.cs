using UnityEngine;
using System.Collections;

namespace Scripts.Calendar.Todos.Interaction
{
    /// <summary>
    /// 할일 ui 자세히 보기 애니메이션 클래스
    /// </summary>
    public class TodoExpander : MonoBehaviour
    {
        public RectTransform todoMoreUI;  // 상세 UI
        public Canvas canvas;             // UI 루트
        public GameObject popup;          // 팝업 루트
        public GameObject todoClone;

        public float duration = 0.3f;

        public void StartExpand(RectTransform fromItem)
        {
            StartCoroutine(ExpandTodo(fromItem));
        }

        private IEnumerator ExpandTodo(RectTransform fromItem)
        {
            GameObject clone = Instantiate(todoClone, canvas.transform);
            clone.SetActive(true);

            RectTransform cloneRect = clone.GetComponent<RectTransform>();
            cloneRect.pivot = cloneRect.anchorMin = cloneRect.anchorMax = new Vector2(0.5f, 0.5f);

            Vector2 startSize = fromItem.rect.size;
            Vector2 endSize = todoMoreUI.rect.size;

            Vector2 screenStart = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, fromItem.position);
            Vector2 screenEnd = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, todoMoreUI.position);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenStart, canvas.worldCamera, out Vector2 localStart);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenEnd, canvas.worldCamera, out Vector2 localEnd);

            cloneRect.anchoredPosition = localStart;
            cloneRect.sizeDelta = startSize;

            float time = 0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                t = t * t * (3f - 2f * t); // SmoothStep

                cloneRect.anchoredPosition = Vector2.Lerp(localStart, localEnd, t);
                cloneRect.sizeDelta = Vector2.Lerp(startSize, endSize, t);
                yield return null;
            }

            popup.SetActive(true);
            todoMoreUI.gameObject.SetActive(true);
            Destroy(clone);
        }
    }
}