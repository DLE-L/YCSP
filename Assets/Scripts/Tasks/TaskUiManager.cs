using TMPro;
using UnityEngine;
using Scripts.AllData;
using Scripts.Calendar.Date;
using Scripts.Calendar.Todos;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using Utils;

namespace Scripts.Tasks
{
  /// <summary>
  /// 일정 ui관련 매니저
  /// </summary>
  public class TaskUiManager : UiManager
  {
    public static TaskUiManager Instance;
    [SerializeField] private RectTransform _taskList;
    [SerializeField] private GameObject _taskPopup;
    [SerializeField] private GameObject _sheetUrl;
    [SerializeField] private GameObject _deleteText;
    [SerializeField] private TextMeshProUGUI _errorPopup;
    [SerializeField] private Canvas _calendarCanvas;
    [SerializeField] private Canvas _taskCanvas;
    public ClipboardManager clipboardManager;
    private TMP_InputField _inputField;
    private string errorMessage;
    public override Dictionary<string, GameObject> uiData { get; set; } = new();
    public override Stack<GameObject> OpenUis { get; set; } = new();

    public TaskItem taskItem;
    private void Start()
    {
      Instance = this;
      _inputField = clipboardManager.inputField;
      _taskPopup.SetActive(false);
      _errorPopup.gameObject.SetActive(false);

      _calendarCanvas.enabled = false;

      StartCoroutine(TaskUpdate());
      
      uiData = new()
      {
        { "SheetUrl", _sheetUrl },
        { "DeleteText", _deleteText },
      };  
    }

    public async void AddTask()
    {
      var dataManager = DataManager.Instance;
      string url = _inputField.text;
      errorMessage = await dataManager.Task.AddNewTask(url);
      if (errorMessage != null)
      {
        ShowErrorPopup();
        return;
      }

      StartCoroutine(TaskUpdate());
    }

    private IEnumerator TaskUpdate()
    {
      DataManager dataManager = DataManager.Instance;
      List<TaskData> taskData = dataManager.Task.Tasks;

      yield return ReturnGameObject();

      for (int i = 0; i < dataManager.Task.Tasks.Count; i++)
      {
        var tempItem = dataManager.Pool.Get<TaskItem>(_taskList);
        var tempObj = tempItem.gameObject;
        tempItem.UpdateTaskItem(taskData[i]);
        tempObj.name = "TaskItem_" + tempItem.taskData.TaskId;
      }
    }

    private IEnumerator ReturnGameObject()
    {
      int childCount = _taskList.childCount;
      for (int i = 1; i < childCount; i++)
      {
        var taskItem = _taskList.GetChild(1).GetComponent<TaskItem>();
        yield return taskItem.ResetData();
      }
      yield return null;
    }

    public void RemoveTaskItem(TaskItem taskItem)
    {
      var dataManager = DataManager.Instance;
      dataManager.Task.RemoveTaskData(taskItem);
      taskItem.ResetData();
    }

    public override void OpenPopup(string uiName, string selfName = null)
    {
      base.OpenPopup(uiName);
      foreach (var item in uiData)
      {
        if (item.Key != uiName)
        {
          item.Value.SetActive(false);
        }
      }
      _taskPopup.SetActive(true);
    }

    public override void ClosePopup()
    {
      base.ClosePopup();
      _taskPopup.SetActive(false);
      _inputField.text = null;
      taskItem = null;
    }

    public void CloseErrorPopup()
    {
      _errorPopup.gameObject.SetActive(false);
      _errorPopup.text = string.Empty;
    }
    public void ShowErrorPopup()
    {
      _errorPopup.gameObject.SetActive(true);
      _errorPopup.text = errorMessage;
    }

    public void OpenCalendar()
    {
      var dataManager = DataManager.Instance;
      var calendarManager = CalendarUiManager.Instance;
      var todoUiManager = TodoUiManager.Instance;

      calendarManager.ShowCalendarUi();
      StartCoroutine(todoUiManager.TodoItemUpdate());

      _calendarCanvas.enabled = true;
      _taskCanvas.enabled = false;
    }

    private void Update()
    {
#if UNITY_ANDROID
      if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
      {
        Application.Quit();
      }
#endif
    }

    public void SetCanvasRaycast(bool active)
    {
      var taskCanvas = _taskCanvas.GetComponent<GraphicRaycaster>();
      taskCanvas.enabled = active;
    }
  }
}