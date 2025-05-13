using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Tasks.Interaction
{
    public class Accept : MonoBehaviour, IPointerDownHandler
    {
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            TaskUiManager uiManager = TaskUiManager.Instance;
            var taskItem = uiManager.GetTaskItem();
            GameObject target = TaskUiManager.Instance.OpenUis.Peek();

            if (target == null) return;

            if (target.name.Equals("SheetUrl"))
            {                
                uiManager.AddTask();
            }
            else if (taskItem.GetComponent<TaskItem>() != null)
            {
                uiManager.RemoveTaskItem(taskItem);
                uiManager.SetTaskItem(null);
            }

            uiManager.ClosePopup();
        }
    }
}