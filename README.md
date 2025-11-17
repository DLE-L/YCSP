# YSCP 일정 관리 앱 (YSCP Schedule Management App)

> 💡 **프로젝트의 핵심:**
> 서울시 청년도전지원사업(YSCP) 참여 당시, 비개발자인 관리자님과 교육생 동료들의 불편함을 해결하기 위해 **자발적으로 기획하고 개발한** Unity 기반 일정 관리 앱입니다.
>
> **'기술은 결국 사람을 향해야 한다'** 는 바탕으로, 전문 기술이 아닌 사용자의 편의성을 최우선으로 고려하여 설계했습니다.

<br>

## 📌 1. 프로젝트 개요

* **프로젝트명:** YSCP 일정 관리 앱
* **개발 기간:** 약 1.5개월 (기획, 개발, 배포, 피드백 반영 및 리팩토링 포함)
* **개발 목표:**
    1.  카카오톡으로 흩어지는 일정 및 과제 공지를 통합 관리
    2.  **비개발자 관리자님**이 코딩 없이 쉽게 일정을 수정할 수 있는 시스템 제공
    3.  교육생들이 날짜별 일정과 과제를 한눈에 파악할 수 있는 UI 제공

<br>

## ✨ 2. 핵심 기능

* **커스텀 캘린더 UI:** 월/일/년 단위로 이동하며 해당 날짜의 일정을 확인(캘린더 패키지 사용)
* **Google Sheet 연동:** 관리자가 Google Sheet만 수정하면, 앱 사용자가 '새로고침' 버튼으로 모든 일정을 즉시 동기화
* **과제/일정 관리:** 날짜별로 '할 일(Todo)'과 '제출 과제(Task)'를 구분하여 표시
* **UI 최적화:** `Object Pooling`을 적용하여 캘린더의 'Day' 오브젝트와 'Todo', 'Task' 리스트 아이템을 재활용

<br>

## 🛠 3. 사용 기술

* **Engine:** Unity
* **Language:** C#
* **Data:** JSON (Google Sheet Export 데이터 파싱)
* **Network:** `UnityWebRequest` (Google Sheet 데이터 로드)
* **UI:** UGUI (커스텀 캘린더, 동적 리스트)
* **Design Patterns:** Singleton (DataManager), Object Pooling
* **Tools:** GitHub

<br>

## 🚀 4. 핵심 시스템 상세 설계

**'사용자(특히 비개발자 관리자)의 편의성'** 이라는 목표를 기준으로 이루어졌습니다.

### 4-1. '사람'을 공감하는 데이터 연동 (Google Sheet Export)

* **파일:** `Assets/Scripts/Calendar/Todos/Interaction/LoadGoogle.cs`, `Assets/Scripts/AllData/DataManager.cs`
* **문제:** 관리자님은 비개발자였기에, DB나 복잡한 CMS를 제공하는 것은 실용적이지 않았습니다. 그분에게 가장 익숙한 도구는 '스프레드시트'였습니다.
* **해결:**
    1.  관리자님이 Google Sheet에 정해진 양식대로 일정을 입력하도록 가이드했습니다.
    2.  `LoadGoogle.cs` 스크립트는 **Google Sheet의 '웹에 게시(CSV/TSV Export)' 기능**을 활용합니다.
    3.  앱은 `UnityWebRequest`를 통해 이 Export URL에 접근하여 텍스트 데이터를 받아옵니다.
    4.  `DataManager.cs`가 이 데이터를 파싱(Parsing)하여 `CalendarList`, `TaskList`, `TodoList`로 가공한 뒤, JSON으로 로컬에 저장합니다.
    5.  사용자가 '새로고침'을 누를 때마다 이 과정을 반복하여 데이터를 최신화합니다.

* **기대 효과:**
    * **관리자:** 개발자의 도움 없이, 언제 어디서든 익숙한 Google Sheet만 수정하면 전체 앱의 일정을 변경할 수 있습니다.
    * **개발자:** DB나 서버 구축/관리 비용 없이, 안정적인 데이터 동기화 기능을 구현했습니다.

### 4-2. 13개의 시트를 1개로: 피드백 기반 리팩토링

* **문제:** 초기 버전은 '1월', '2월'... '12월', '과제' 등 **13개의 시트**를 참조했습니다. 관리자님이 "시트가 너무 많아 관리하기 번거롭다"는 피드백을 주셨습니다.
* **해결:**
    1.  데이터 구조를 근본적으로 변경했습니다. 13개로 나뉘어 있던 시트를 **'통합 일정' 단일 시트**로 리팩토링했습니다.(과제 수가 적은 것에 반영)
    2.  `LoadGoogle.cs`의 파싱 로직을 수정하여, 단일 시트에서 날짜(Key)를 기준으로 데이터를 읽어와 `DataManager`에 적재하도록 변경했습니다.
* **결과:** 관리자의 작업 동선을 획기적으로 개선했으며, 사용자 피드백을 수용하여 실제 사용성을 개선한 반복(Iteration) 개발 프로세스를 경험했습니다.

### 4-3. UI 성능 최적화 (Object Pooling)

* **파일:** `Assets/Scripts/AllData/PoolList.cs`, `Assets/Scripts/Calendar/Date/CalendarUiManager.cs`
* **문제:** 캘린더는 31일(Day prefab)을, 일정 리스트는 수십 개의(TodoItem/TaskItem prefab) UI 오브젝트를 사용합니다. 매월 또는 매일 데이터를 새로고침할 때마다 `Instantiate`와 `Destroy`를 반복하면 성능 저하가 발생합니다.
* **해결:**
    1.  `PoolList.cs`라는 간단한 오브젝트 풀링 매니저를 구현했습니다.
    2.  `CalendarUiManager`는 `Day` 프리팹을, `TodoUiManager`는 `TodoItem` 프리팹을 `PoolList`에서 `Get()`하여 사용합니다.
    3.  날짜가 변경되거나 새로고침 시, 기존에 사용한 오브젝트를 `Destroy`하지 않고 `Release()`하여 풀에 반납합니다.
* **결과:** 앱 전체의 UI 반응 속도를 높이고 가비지 컬렉션(GC)을 최소화했습니다.