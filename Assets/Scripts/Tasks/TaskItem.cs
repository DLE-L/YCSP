using Scripts.AllData;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Scripts.Tasks
{
  /// <summary>
  /// 일정 아이템 클래스
  /// </summary>
  public class TaskItem : MonoBehaviour, IOpenAble
  {
    [SerializeField] private TextMeshProUGUI titleName;
    [SerializeField] private TextMeshProUGUI startDate;
    [SerializeField] private TextMeshProUGUI endDate;
    public TaskData taskData;

    private bool _isHolding;  // 버튼이 눌려 있는지 여부
    private float _holdTime; // 버튼이 눌린 시간 추적        

    public string UiName => "DeleteText";

    void Update()
    {
      if (InteractionManager.HoldingOneSeconds(ref _isHolding, ref _holdTime))
      {
        var taskUiManager = TaskUiManager.Instance;
        taskUiManager.OpenPopup(UiName);
        taskUiManager.taskItem = this;
      }
    }

    public void UpdateTaskItem(TaskData task)
    {
      taskData = task;
      titleName.text = taskData.TitleName;
      startDate.text = taskData.StartDate.Start;
      endDate.text = taskData.EndDate.End;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
      _isHolding = true;
      _holdTime = 0f;
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
      _isHolding = false;
      _holdTime = 0f;
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
      var dataManager = DataManager.Instance;
      var taskUiManager = TaskUiManager.Instance;
      dataManager.Todo.CurrentTaskId = taskData.TaskId;
      taskUiManager.OpenCalendar();
    }

    public void ResetData()
    {
      var dataManager = DataManager.Instance;
      taskData = null;
      titleName.text = null;
      startDate.text = null;
      endDate.text = null;
      dataManager.Pool.Return<TaskItem>(gameObject);
    }
  }
}