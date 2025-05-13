using TMPro;
using UnityEngine;
using Scripts.AllData;
using Scripts.Calendar.Date;
using Scripts.Calendar.Todos;
using Utils;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

        private GameObject _currentTaskItem;

        private void Start()
        {
            Instance = this;
            _inputField = clipboardManager.inputField;
            _taskPopup.SetActive(false);
            _errorPopup.gameObject.SetActive(false);

            _calendarCanvas.enabled = false;
            TaskUpdate();
            uiData = new()
            {
                {"SheetUrl", _sheetUrl},
                {"DeleteText", _deleteText},
            };
        }

        public async void AddTask()
        {
            string url = _inputField.text;
            errorMessage = await DataManager.Instance.Task.AddNewTask(url);
            if (errorMessage != null)
            {
                ShowErrorPopup();
                return;
            }

            TaskUpdate();
        }

        private void TaskUpdate()
        {
            DataManager dataManager = DataManager.Instance;
            var data = dataManager.Task.Tasks;
            for (int i = 0; i < dataManager.Task.Tasks.Count; i++)
            {
                GameObject obj = Instantiate(dataManager.itemInfo.taskItem.gameObject, _taskList);
                obj.name = "TaskItem_" + i;
                TaskItem tempItem = obj.GetComponent<TaskItem>();
                tempItem.taskData = data[i];
                tempItem.titleName.text = data[i].TitleName;
            }
        }

        public void RemoveTaskItem(GameObject taskItem)
        {
            DataManager.Instance.Task.RemoveTaskData(taskItem);
            Destroy(taskItem);
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

        public GameObject GetTaskItem()
        {
            return _currentTaskItem;
        }
        public void SetTaskItem(GameObject taskItem)
        {
            _currentTaskItem = taskItem;
        }

        public override void ClosePopup()
        {
            base.ClosePopup();
            _taskPopup.SetActive(false);
            _inputField.text = null;
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
            _calendarCanvas.enabled = true;
            _taskCanvas.enabled = false;
            CalendarUiManager.Instance.ShowCalendarUi();
            StartCoroutine(TodoUiManager.Instance.TodoItemUpdate());
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
            _taskCanvas.GetComponent<GraphicRaycaster>().enabled = active;
        }
    }
}