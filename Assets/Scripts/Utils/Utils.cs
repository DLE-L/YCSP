using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Utils
{
    /// <summary>
    /// 클릭시 여는 Ui 인터페이스
    /// </summary>
    public interface IOpenAble : IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        public string UiName { get; }
    }

    /// <summary>
    /// UiManager들 각각 ui를 Stack으로 관리
    /// </summary>
    public abstract class UiManager : MonoBehaviour
    {
        public abstract Dictionary<string, GameObject> uiData { get; set; }
        public abstract Stack<GameObject> OpenUis { get; set; }
        public virtual void OpenPopup(string uiName, string selfName = null)
        {
            if (uiData.TryGetValue(uiName, out var gameObject))
            {
                gameObject.SetActive(true);
                OpenUis.Push(gameObject);
            }
        }
        public virtual void ClosePopup()
        {
            if (OpenUis.Count < 1) return;
            var gameObject = OpenUis.Pop();
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 상호작용 관련 매니저
    /// </summary>
    public static class InteractionManager
    {
        /// <summary>
        /// 오브젝트를 duration초 이상 누르고 있는지 확인하는 함수
        /// </summary>
        public static bool HoldingSeconds(ref bool isHolding, ref float holdTime, float duration)
        {
            if (isHolding)
            {
                holdTime += Time.deltaTime; // 버튼을 누르고 있는 동안 경과 시간 추가

                if (holdTime >= duration)
                {
                    isHolding = false; // 더 이상 카운트하지 않음
                    return true;
                }
            }
            else
            {
                holdTime = 0f; // 초기화
            }

            return false;
        }
        public static bool HoldingOneSeconds(ref bool isHolding, ref float holdTime, GameObject targetGameObject = null)
        {
            // GameObject가 비활성화 상태라면 바로 false 반환
            if (targetGameObject != null && !targetGameObject.activeSelf)
            {
                return false;
            }

            // 1초 이상 눌렀는지 확인
            return HoldingSeconds(ref isHolding, ref holdTime, 1f);
        }
        public static bool HoldingOneSeconds(ref bool isHolding, ref float holdTime)
        {
            return HoldingSeconds(ref isHolding, ref holdTime, 1f);
        }

        public static bool Click(ref bool isHolding, ref float holdTime)
        {
            if (isHolding)
            {
                holdTime += Time.deltaTime; // 버튼을 누르고 있는 동안 경과 시간 추가

                if (holdTime < 0.05f)
                {
                    isHolding = false; // 더 이상 카운트하지 않음
                    return true;
                }
            }
            else
            {
                holdTime = 0f; // 초기화
            }

            return false;
        }
    }

    /// <summary>
    /// Json저장, 로드관련 매니저
    /// </summary>
    public static class JsonManager
    {
        public static T LoadJson<T>(string jsonName)
        {
            string filePath = Application.persistentDataPath + "/" + jsonName + ".json";

            if (File.Exists(filePath) == false)
            {
                CreateJson(jsonName); // 파일이 없으면 새로 생성
                return default;
            }

            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<T>(json);
        }

        public static void SaveJson<T>(string jsonName, T data)
        {
            string json = JsonUtility.ToJson(data, true);
            string jsonPath = Application.persistentDataPath + "/" + jsonName + ".json";
            File.WriteAllText(jsonPath, json);
        }

        private static void CreateJson(string jsonName)
        {
            string jsonPath = Application.persistentDataPath + "/" + jsonName + ".json";
            using (FileStream fs = File.Create(jsonPath))
            {

            }
        }
    }

    /// <summary>
    /// 구글 시트 데이터 관련 매니저
    /// </summary>
    public static class GoogleSheetManager
    {
        private const string EXPORT_URL = "/export?format=csv&";
        public static string Export_Url { get { return EXPORT_URL; } }

        public static async Task<string> LoadCSVData(string url)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                var operation = webRequest.SendWebRequest();

                while (!operation.isDone) await Task.Yield();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    return webRequest.downloadHandler.text;
                }
                else
                {
                    Debug.LogError($"Error Downloading CSV: {webRequest.error}");
                    return null;
                }
            }
        }

        public static string GetSheetUrl(string url)
        {
            // URL 유효성 검사
            bool isValidUrl = !string.IsNullOrEmpty(url) && url.Contains("/d/");
            if (!isValidUrl)
            {
                return null;
            }
            string gid = GetGoogleSheetUrlID(url);
            string baseUrl = GetGoogleSheetUrlBase(url);
            url = baseUrl + Export_Url + gid;
            return url;
        }

        private static string GetGoogleSheetUrlBase(string url)
        {
            int startIndex = url.IndexOf("/d/") + 3;
            int endIndex = url.IndexOf('/', startIndex);
            if (endIndex > startIndex)
            {
                url = url.Substring(0, endIndex);
            }
            return url;
        }

        private static string GetGoogleSheetUrlID(string url)
        {
            Match match = Regex.Match(url, @"gid=(\d+)");
            if (match.Success)
            {
                return match.Groups[0].Value;
            }
            return null;
        }
    }
}