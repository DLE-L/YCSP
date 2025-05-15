using UnityEngine;
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
		[NonSerialized] private Dictionary<string, int> TaskIdLookup = new();
		[NonSerialized] private Dictionary<string, (DateTime, DateTime)> RangeDateLookup = new();
		public List<TaskData> Tasks = new();

		private void Save(string fileName = "Tasks")
		{
			JsonManager.SaveJson<TaskManager>(fileName, this);
		}

		public TaskManager Load(string fileName = "Tasks")
		{
			TaskManager tempList = JsonManager.LoadJson<TaskManager>(fileName);
			if (tempList != null)
			{
				Tasks = tempList.Tasks;
				SetLookupTable();
			}
			return tempList ?? new TaskManager();
		}

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
				TodoManager = new(),
			};
			taskData.TodoManager.InitTodoManager(csvData, taskData.TaskId);

			Tasks.Add(taskData);
			if (!TaskIdLookup.ContainsKey(taskData.TaskId))
			{
				TaskIdLookup[taskData.TaskId] = Tasks.Count - 1;
			}

			if (!RangeDateLookup.ContainsKey(taskData.TaskId))
			{
				RangeDateLookup[taskData.TaskId] = (taskData.StartDate.Date, taskData.EndDate.Date);
			}

			Save();
		}

		public (DateTime, DateTime) GetTaskDateRange(string taskId)
		{
			return RangeDateLookup[taskId];
		}

		public void RemoveTaskData(TaskItem taskItem)
		{
			string taskId = taskItem.taskData.TaskId;
			int index = TaskIdLookup[taskId];
			Tasks.RemoveAt(index);
			TaskIdLookup.Remove(taskId);

			Save();
		}

		public void SetLookupTable()
		{
			TaskIdLookup.Clear();
			int count = 0;
			foreach (var taskData in Tasks)
			{
				var key = taskData.TaskId;
				if (!TaskIdLookup.ContainsKey(key))
				{
					TaskIdLookup[key] = count++;
				}
				if (!RangeDateLookup.ContainsKey(key))
				{
					RangeDateLookup[key] = (taskData.StartDate.Date, taskData.EndDate.Date);
				}
			}
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
		public TodoManager TodoManager = new();
	}
}