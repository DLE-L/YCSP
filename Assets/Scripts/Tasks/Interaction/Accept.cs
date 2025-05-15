using UnityEngine;
using UnityEngine.EventSystems;

namespace Scripts.Tasks.Interaction
{
	public class Accept : MonoBehaviour, IPointerDownHandler
	{
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			var taskUiManager = TaskUiManager.Instance;
			GameObject target = taskUiManager.OpenUis.Peek();

			if (target == null) return;

			if (target.name.Equals("SheetUrl"))
			{
				taskUiManager.AddTask();
			}
			else if (target.name.Equals("DeleteText"))
			{
				TaskItem taskItem = taskUiManager.taskItem;
				taskUiManager.RemoveTaskItem(taskItem);
			}

			taskUiManager.ClosePopup();
		}
	}
}