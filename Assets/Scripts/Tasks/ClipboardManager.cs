using UnityEngine;
using TMPro;

namespace Scripts.Tasks
{
    /// <summary>
    /// 핸드폰 Android, IOS 클립보드 데이터 InputField에 추가하는 매니저
    /// </summary>
    public class ClipboardManager : MonoBehaviour
    {
        public TMP_InputField inputField;

        public string PasteClipboardToInput()
        {
            string clipboardText = GetClipboard();
            if (string.IsNullOrEmpty(clipboardText)) return null;
            inputField.text = clipboardText;
            return clipboardText;
        }

        string GetClipboard()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (var clipboardManager = activity.Call<AndroidJavaObject>("getSystemService", "clipboard"))
        {
            var clipData = clipboardManager.Call<AndroidJavaObject>("getPrimaryClip");
            if (clipData != null && clipData.Call<int>("getItemCount") > 0)
            {
                var item = clipData.Call<AndroidJavaObject>("getItemAt", 0);
                return item.Call<string>("getText");
            }
        }
#elif UNITY_IOS && !UNITY_EDITOR
        return _GetClipboardText();
#else
            return GUIUtility.systemCopyBuffer;
#endif
            return null;
        }

#if UNITY_IOS && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string _GetClipboardText();
#endif
    }
}