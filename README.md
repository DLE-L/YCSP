-----

# YSCP 일정 관리 앱 (Schedule Manager)

> **"기술은 결국 사람을 향해야 한다"** <br>
> 비개발자인 관리자의 편의성을 최우선으로 고려한 Google Sheet 연동 일정 관리 앱 (1인 개발)

## 1. 프로젝트 개요

  * **엔진:** Unity
  * **언어:** C\#
  * **인원:** 1인 개발
  * **개발 기간:** 2025.04 \~ 2025.06 (약 1.5개월)
  * **목표:**
      * 비개발자(관리자)가 별도 CMS 없이 **Google Sheet**만으로 앱 데이터를 관리
      * 13개로 분산된 데이터 구조를 피드백 기반으로 통합 및 최적화
      * 반복되는 리스트 UI(Todo/Task)의 성능 최적화 (Object Pooling)

<br>

## 2. 기술 스택

| 분류 | 기술 | 구현 내용 |
| :--- | :--- | :--- |
| **Network** | **UnityWebRequest** | Google Sheet의 `App Script` 없이 CSV/TSV Export URL을 통해 데이터 수신 |
| **Data** | **JSON** | 파싱된 일정 데이터를 로컬에 직렬화하여 저장 및 로드 |
| **Design Pattern** | **Singleton** | `DataManager`를 통한 전역 데이터 접근 및 관리 |
| **Optimization** | **Object Pooling** | 캘린더 날짜(Day) 및 일정 항목(Item)의 재사용 처리 |
| **UI** | **UGUI / Calendar Package** | 커스텀 캘린더 로직 구현 및 동적 리스트 생성 |

<br>

## 3. 핵심 구현 사항 (Key Implementations)

### 3-1. 비개발자를 위한 데이터 파이프라인 (Google Sheet Sync)

  * **문제:** 관리자가 비개발자이므로 DB나 관리자 페이지(CMS) 사용이 불가능함.
  * **해결:** 관리자에게 익숙한 **Google Sheet를 백엔드 대용으로 사용**하는 파이프라인 구축.
      * Google Sheet의 '웹에 게시(CSV Export)' 기능을 활용.
      * 앱 실행 시 `UnityWebRequest`로 텍스트 데이터를 받아와 파싱 후 `CalendarList`, `TaskList` 객체로 매핑.

<!-- end list -->

```csharp
// LoadGoogle.cs (개념 코드)
IEnumerator LoadScheduleData() {
    string url = "https://docs.google.com/spreadsheets/d/.../export?format=tsv";
    using (UnityWebRequest www = UnityWebRequest.Get(url)) {
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success) {
            string tsvData = www.downloadHandler.text;
            DataManager.Instance.ParseAndSave(tsvData); // 데이터 파싱 및 로컬 저장
        }
    }
}
```

### 3-2. 피드백 기반 데이터 구조 리팩토링

  * **기존:** 월별(1월\~12월) 시트가 13개로 나뉘어 있어 데이터 파싱 및 관리가 복잡.
  * **개선:** 사용자(관리자) 피드백 수용 후 **단일 시트 통합 구조**로 변경.
      * `날짜(Key) | 내용 | 타입` 형태의 단일 테이블로 정규화.
      * 파싱 로직을 단순화하고 유지보수성 대폭 향상.

### 3-3. UI 성능 최적화 (Object Pooling)

  * **문제:** 캘린더의 날짜(30\~31개)와 일정 목록이 갱신될 때마다 `Instantiate/Destroy` 호출 시 프레임 드랍 발생.
  * **해결:** `PoolList.cs`를 통해 UI 오브젝트를 미리 생성하고 재사용.
      * `CalendarUiManager`와 `TodoUiManager`가 풀링 시스템을 통해 오브젝트를 활성/비활성화.

<!-- end list -->

```csharp
// PoolList.cs (간소화)
public GameObject Get(string type) {
    if (poolDict[type].Count > 0) {
        GameObject obj = poolDict[type].Dequeue();
        obj.SetActive(true);
        return obj;
    }
    return Instantiate(prefabs[type]); // 부족하면 생성
}
```

<br>

## 4. 프로젝트 구조

```
Assets/Scripts
├── AllData            # 데이터 구조 정의 및 DataManager (싱글톤)
├── Calendar           # 캘린더 UI 로직 및 날짜 계산
│   └── Todos          # 일정/과제 아이템 처리 및 파싱 로직 (LoadGoogle)
├── Explain            # 앱 사용 가이드/튜토리얼 로직
├── Tasks              # 과제 제출 관련 로직
└── Utils              # 유틸리티 (Object Pooling, Helper)
```
<br>
