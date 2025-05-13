using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Scripts.Explain
{
    public class ExplainUiManager : UiManager
    {
        public static ExplainUiManager Instance;
        [SerializeField] private GameObject _explainSelect;
        [SerializeField] private GameObject _explainUi;
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;

        [SerializeField] private ExplainInfo _google = new();
        [SerializeField] private ExplainInfo _task = new();
        [SerializeField] private ExplainInfo _todo = new();
        [SerializeField] private ExplainInfo _currentInfo;
        [SerializeField] private Canvas _explainCanvas;
        private Dictionary<string, ExplainInfo> _explainTarget = new();
        private int _explainIndex = 0;

        public override Dictionary<string, GameObject> uiData { get; set; } = new();
        public override Stack<GameObject> OpenUis { get; set; } = new();

        void Start()
        {
            Instance = this;            
            _explainSelect.SetActive(false);
            _explainUi.SetActive(false);
            _explainTarget = new() 
            {
                {"Google", _google},
                {"Task", _task},
                {"Todo", _todo},
            };
            _google.explainText = new string[11]
            {
                "1. GoogleDrive\nGoogle스프레드 시트 생성",
                "2. 시트 생성 후 공유 설정\n링크가 있는 모든 사용자 체크",
                "3. 오른쪽 위 설정",
                "4. 공유 옵션 두개 체크\n(기본적으로 체크 되있음)",
                "5. 처음시트 & 12개월 시트 생성",
                "6. 첫 시트 일정 제목 A2셀에 입력",
                "7. B2~B13 월 숫자만 입력",
                "8. C2~C13 각 맞는 \n월 시트 Url 입력",
                "9. C2~C13 시트 Url ctrl+c ctrl+v",
                "10. 각 월 시트 A2부터 날짜 입력\n(예시 \"2025-01-01\")",
                "11. 한 날짜에 할일 2개 이상\n아래 행 삽입 Todo&Note 입력\n(날짜x)",
            };
            _task.explainText = new string[3]
            {
                "1. 일정 추가",
                "2. 저장된 구글 시트 Url 붙여넣기",
                "3. 일정 추가 확인",
            };

            _todo.explainText = new string[4]
            {
                "1. 연도, 월 이동",
                "2. 해당 날짜에 할 일 있을시 표시",
                "3. 할 일 클릭시 자세한 정보",
                "4. 할 일 갱신(공지시 클릭)",
            };

            uiData = new()
            {
                {"ExplainSelect", _explainSelect},
                {"ExplainUi", _explainUi},
            };
        }

        private void OpenExplainUi(string uiName)
        {
            if (!_explainTarget.TryGetValue(uiName, out var info))
            {
                return;
            }
            _currentInfo = info;
            _explainIndex = 0;
            UpdateExplain(0);
            _image.SetNativeSize();
        }

        public void UpdateExplain(int direction)
        {
            int length = _currentInfo.explainSprite.Length - 1;
            _explainIndex = _explainIndex + direction;
            if (_explainIndex < 0 || _explainIndex > length)
            {
                _explainIndex += direction * (-1);
                return;
            }
            _image.sprite = _currentInfo.explainSprite[_explainIndex];
            _text.text = _currentInfo.explainText[_explainIndex];                     
        }

        public override void OpenPopup(string uiName, string selfName = null)
        {
            base.OpenPopup(uiName);
            if (uiName.Equals("ExplainUi"))
            {
                OpenExplainUi(selfName);
            }
        }

        public override void ClosePopup()
        {
            base.ClosePopup();
        }

        public void SetCanvasRaycast(bool active)
        {
            _explainCanvas.GetComponent<GraphicRaycaster>().enabled = active;
        }
    }

    /*
        - Google
        1. Instantiate      : GoogleDrive\nGoogle스프레드 시트 생성
        2. Share_1          : 시트 생성 후 공유 설정\n링크가 있는 모든 사용자 체크
        3. Share_2          : 오른쪽 위 설정
        4. Share_3          : 공유 옵션 두개 체크\n(기본적으로 체크 되있음)
        5. Sheet            : 처음시트 & 12개월 시트 생성
        6. FirstSheet_Title : 첫 시트 일정 제목 A2셀에 입력
        7. FirstSheet_Month : B2~B13 월 숫자만 입력
        8. FirstSheet_Url   : C2~C13 각 맞는 월 시트 Url 입력
        9. FirstSheet_AddUrl: C2~C13 시트 Url ctrl+c ctrl+v
        10. Sheet_Date      : 각 월 시트 A2부터 날짜 입력\n(예시 "2025-01-01")
        11. Sheet_Todo_Note : 한 날짜에 할일 2개 이상일 경우\n행 삽입 후 할일만 입력
        
        - Task
        1. 
        2. 
        3. 

        - Todo

    */

    [System.Serializable]
    public class ExplainInfo
    {
        public Sprite[] explainSprite;
        public string[] explainText;
    }
}