using UnityEngine;
using Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scripts.AllData
{
    [System.Serializable]
    public class TaskList
    {
        public int Index = 0;
        public List<TaskData> Tasks = new List<TaskData>();

        private void Save(string fileName = "Tasks")
        {
            JsonManager.SaveJson<TaskList>(fileName, this);
        }

        public TaskList Load(string fileName = "Tasks")
        {
            TaskList tempList = JsonManager.LoadJson<TaskList>(fileName);
            if (tempList != null)
            {
                Index = tempList.Index;
                Tasks = tempList.Tasks;
            }
            return tempList ?? new TaskList();
        }

        public async Task<string> AddNewTask(string url)
        {
            if (!url.Contains("gid=0"))
            {
                return "첫 시트가 아닙니다";
            }

            string exportUrl = GoogleSheetManager.GetSheetUrl(url);
            if (string.IsNullOrEmpty(exportUrl))
            {
                return "주소를 제대로 입력해 주세요";
            }

            bool isDuplicate = Tasks.Find(task => task.TaskUrl == exportUrl) != null;
            if (Index > 0 && isDuplicate)
            {
                return "중복된 일정 입니다";
            }


            string csvData = await GoogleSheetManager.LoadCSVData(exportUrl);

            // 데이터 유무 확인
            if (string.IsNullOrEmpty(csvData))
            {
                return "데이터를 가져오지 못했습니다";
            }

            SaveTaskData(csvData, exportUrl);

            return null;
        }

        private void SaveTaskData(string csvData, string taskUrl)
        {
            // JSON 데이터 행 열
            string[] rows = csvData.Split('\n');           

            // 제목 할당
            string[] titleRow = rows[1].Split(',');
            string titleName = titleRow[0].Trim();

            // 월별 주소 할당
            List<UrlData> sheetUrl = new List<UrlData>(12);
            UrlData urlData;
            for (int i = 1; i <= sheetUrl.Capacity; i++)
            {
                string[] row = rows[i].Split(',');
                string url = GoogleSheetManager.GetSheetUrl(row[2]);
                urlData = new UrlData
                {
                    Month = int.Parse(row[1].Trim()),
                    Url = url.Trim(),
                };
                sheetUrl.Add(urlData);
            }

            TaskData taskData = new TaskData
            {
                TaskUrl = taskUrl,
                TitleName = titleName,
                Urls = sheetUrl,
            };
            
            Tasks.Add(taskData);
            Index++;

            Save();
        }

        public void RemoveTaskData(GameObject taskItem)
        {
            string[] stringIndex = taskItem.name.Split('_');
            int index = int.Parse(stringIndex[1].Trim());

            DataManager.Instance.TodoList.RemoveTodoData(Tasks[index]);
            Tasks.RemoveAt(index);
            
            Index--;

            Save();
        }
    }

    [System.Serializable]
    public class TaskData
    {
        public string TaskUrl;
        public string TitleName;
        public List<UrlData> Urls = new List<UrlData>();
    }

    [System.Serializable]
    public class UrlData
    {
        public int Month;
        public string Url;
    }
}

