using Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Scripts.Tasks;

namespace Scripts.AllData
{
	[System.Serializable]
	public class TaskManager
	{
		[NonSerialized] public string CurrentTaskId;
		[NonSerialized] private Dictionary<string, int> TaskIdLookup = new();
		[NonSerialized] private Dictionary<string, TaskData> DataLookup = new();
		[NonSerialized] private Dictionary<string, (DateTime, DateTime)> RangeDateLookup = new();
		public List<TaskData> Tasks = new();

		public async Task<string> AddNewTask(string url)
		{
			string exportUrl = GoogleSheetManager.GetSheetUrl(url);
			if (string.IsNullOrEmpty(exportUrl))
			{
				return "주소를 제대로 입력해 주세요";
			}

			bool isDuplicate = Tasks.Find(task => task.TaskUrl == exportUrl) != null;
			if (Tasks.Count > 0 && isDuplicate)
			{
				return "중복된 일정 입니다";
			}

			string csvData = await GoogleSheetManager.LoadCSVData(exportUrl);

			// 데이터 유무 확인
			if (string.IsNullOrEmpty(csvData))
			{
				return "데이터를 가져오지 못했습니다";
			}

			InitTaskManager(csvData, exportUrl);

			return null;
		}

		private void InitTaskManager(string csvData, string taskUrl)
		{
			// JSON 데이터 행 열
			string[] rows = csvData.Split('\n');

			// 제목 할당
			string[] rowOne = rows[0].Split(',');
			string titleName = rowOne[0].Trim();

			// 할일 할당
			StartDate startDate = new(rowOne[1].Trim());
			EndDate endDate = new(rowOne[2].Trim());

			TaskData taskData = new TaskData
			{
				StartDate = startDate,
				EndDate = endDate,
				TaskUrl = taskUrl,
				TitleName = titleName,
			};
			Tasks.Add(taskData);

			var dataManager = DataManager.Instance;
			TodoManager todoManager = dataManager.Todo;
			todoManager.UpdateTodoManager(csvData, taskData.TaskId);

			SetLookupTable();
			Save();
		}

		public async Task UpdateTaskData()
		{
			if (!DataLookup.ContainsKey(CurrentTaskId)) return;

			TaskData taskData = DataLookup[CurrentTaskId];

			string taskUrl = taskData.TaskUrl;
			string csvData = await GoogleSheetManager.LoadCSVData(taskUrl);

			string[] rows = csvData.Split('\n');
			string[] rowOne = rows[0].Split(',');

			// 기존 TaskData의 속성 업데이트 (TaskId는 변경하지 않음)
			taskData.TitleName = rowOne[0].Trim();
			taskData.StartDate = new StartDate(rowOne[1].Trim());
			taskData.EndDate = new EndDate(rowOne[2].Trim());

			var dataManager = DataManager.Instance;
			TodoManager todoManager = dataManager.Todo;
			todoManager.UpdateTodoManager(csvData, taskData.TaskId);

			SetLookupTable();
			Save();
		}

		public (DateTime, DateTime) GetTaskDateRange(string taskId)
		{
			return RangeDateLookup[taskId];
		}

		public void SetLookupTable()
		{
			TaskIdLookup.Clear();
			RangeDateLookup.Clear();
			DataLookup.Clear();
			int count = 0;
			foreach (var taskData in Tasks)
			{
				var key = taskData.TaskId;

				DataLookup[key] = taskData;
				TaskIdLookup[key] = count++;
				RangeDateLookup[key] = (taskData.StartDate.Date, taskData.EndDate.Date);
			}
		}

		public void RemoveTaskData(TaskItem taskItem)
		{
			var dataManager = DataManager.Instance;
			TodoManager todoManager = dataManager.Todo;
			TodoCompleteManager completeManager = dataManager.Complete;
			string taskId = taskItem.taskData.TaskId;

			int index = TaskIdLookup[taskId];
			Tasks.RemoveAt(index);
			completeManager.RemoveTodoComplete(taskId);
			todoManager.RemoveTodo(taskId);

			SetLookupTable();
			Save();
		}

		private void Save(string fileName = "Tasks")
		{
			JsonManager.SaveJson<TaskManager>(fileName, this);
		}

		public TaskManager Load(string fileName = "Tasks")
		{
			var tempList = JsonManager.LoadJson<TaskManager>(fileName);
			if (tempList != null)
			{
				Tasks = tempList.Tasks;
				SetLookupTable();
			}
			return tempList ?? new TaskManager();
		}
	}

	[System.Serializable]
	public class TaskData
	{
		public StartDate StartDate;
		public EndDate EndDate;
		public string TaskUrl;
		public string TitleName;
		public string TaskId = Guid.NewGuid().ToString();
	}
}