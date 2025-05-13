using UnityEngine;
using Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Scripts.AllData
{
    [System.Serializable]
    public class TaskManager
    {
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

            SaveTask(csvData, exportUrl);

            return null;
        }

        private void SaveTask(string csvData, string taskUrl)
        {
            // JSON 데이터 행 열
            string[] rows = csvData.Split('\n');

            // 제목 할당
            string[] rowOne = rows[0].Split(',');
            string titleName = rowOne[0].Trim();
            string[] start = rowOne[1].Split('-');
            string[] end = rowOne[2].Split('-');
            StartDate startDate;
            {
                int year = int.Parse(start[0]);
                int month = int.Parse(start[1]);
                int day = int.Parse(start[2]);
                startDate = new(year, month, day);
            }
            EndDate endDate;
            {
                int year = int.Parse(end[0]);
                int month = int.Parse(end[1]);
                int day = int.Parse(end[2]);
                endDate = new(year, month, day);
            }

            TaskData taskData = new TaskData
            {
                TaskStartDate = startDate,
                TaskEndDate = endDate,
                TaskUrl = taskUrl,
                TitleName = titleName,
                TodoManager = new(),
            };
            taskData.TodoManager.InitTodoManager(csvData);
            taskData.TodoId = taskData.TodoManager.TodoManagerId;
            Tasks.Add(taskData);

            Save();
        }

        public void RemoveTaskData(GameObject taskItem)
        {
            string[] stringIndex = taskItem.name.Split('_');
            int index = int.Parse(stringIndex[1].Trim());
            Tasks.RemoveAt(index);

            Save();
        }
    }

    [System.Serializable]
    public class TaskData
    {
        public StartDate TaskStartDate;
        public EndDate TaskEndDate;
        public string TaskUrl;
        public string TitleName;
        public string TaskId = Guid.NewGuid().ToString();
        public string TodoId;
        [NonSerialized] public TodoManager TodoManager = new();
    }
}