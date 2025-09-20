

# 📂 UI 폴더 구조 및 클래스 간략 설명

### 📌 개요

게임의 모든 사용자 인터페이스(UI) 관련 스크립트를 관리하는 폴더입니다.
MVC(Model-View-Controller) 디자인 패턴에 기반해 UI의 시각적 요소와 데이터 표시를 담당하는 View와, 이 View들의 상태와 상호작용을 제어하는 Manager/Controller의 역할을 명확하게 분리하여 관리합니다. 이를 통해 UI 로직의 복잡성을 줄이고 유지보수성을 높입니다.

---

## 📁 폴더 구조 및 역할

* **`UIView.cs`**
  * 모든 UI를 구성하는 View의 기본 베이스가 되는 추상 클래스 입니다. Fade기반 Awake, Show/Hide 전환에 대한 기본적인 애니메이션 동작 기능을 포함하고, 자식클래스에는 이 기능을 상속받고 override하여 선택적으로 기능을 확장할 수 있도록 합니다.
* **`QuitView.cs`**
  * LobbyScene에서 게임을 종료하기 위한 View로, UIView를 상속 받습니다. 종료버튼을 누르면 Application.Quit()을 호출해 게임이 즉시 종료되고, 취소버튼을 누르면 이전 View로 전환됩니다.
* **`SettingsView.cs`**
  * BGM, SFX 사운드 볼륨을 설정하기 위한 View로, UIView를 상속 받습니다. 각 볼륨은 Slider를 통해 조절되며 변경값은 SoundManager에 전달되어 PlayerPrefs를 통해 저장됩니다. SettingsView 컴포넌트를 가진 실제 View 오브젝트를 프리팹화하여 Lobby, 각 Stage 씬에서 재사용하는 방식으로 일관된 View 표시를 보장합니다.

### 🔹 **Lobby**

> 게임의 메인 로비와 스테이지 선택 화면 등, 게임 시작 전에 사용되는 UI를 관리하는 폴더입니다.

* **`LobbyUIManager.cs`**
  * 로비 씬의 UI 패널(`LobbyView`, `StageListView`) 간의 전환을 책임지는 컨트롤러(Controller) 클래스입니다. `ShowView()` 메서드를 통해 현재 활성화할 View를 관리하여, 로비와 스테이지 목록 간의 화면 이동 로직을 중앙에서 처리합니다.
* **`LobbyView.cs`**
  * 메인 로비 화면에 표시되는 UI 요소들을 관리하는 뷰(View) 클래스입니다. 재화, 버전 정보 텍스트와 스테이지 목록으로 이동하는 버튼의 이벤트를 처리합니다.
* **`StageListView.cs`**
  * 스테이지 선택 목록 화면을 관리하는 뷰(View) 클래스입니다. `StageData`를 기반으로 `StageItemView` 프리팹을 동적으로 생성하고, PlayerPrefs에서 저장된 별점을 불러와 각 스테이지의 클리어 상태를 표시합니다.
* **`StageItemView.cs`**
  * 스테이지 목록에 표시될 개별 스테이지 아이템의 UI를 담당하는 재사용 가능한 뷰(View) 컴포넌트입니다. 스테이지 이름, 썸네일, 별점을 표시하고 'Play' 버튼 클릭 시 해당 스테이지 씬을 로드하는 역할을 합니다.
* **`StageData.cs`**
  * 각 스테이지의 정보를 담는 데이터 모델(Model) 클래스입니다. [System.Serializable] 속성을 통해 인스펙터에서 쉽게 데이터를 설정할 수 있으며, 스테이지 인덱스, 이름, 썸네일, 별점과 같은 순수 데이터를 보관합니다.

---

### 🔹 **Stage**

> 실제 인게임 플레이 중에 표시되는 UI(HUD, 일시정지, 결과 창 등)를 관리하는 폴더입니다.

* **`StageUIManager.cs`** 
	-   인게임 씬에 존재하는 여러 UI **View**(`StageHUDView`, `StagePauseView`, `StageResultView` 등)들의 활성화 상태를 총괄하는 상위 **매니저(Manager)** 클래스입니다.    
	-   예를 들어, 일시정지 버튼이 눌리면 게임 플레이 HUD를 숨기고 일시정지 메뉴를 표시하는 것처럼, 상황에 따라 어떤 UI 그룹을 보여주고 숨길지를 결정하는 역할을 담당합니다. `StageUIController`가 데이터 흐름을 제어한다면, `StageUIManager`는 **UI 패널 간의 전환과 흐름**을 제어합니다.

*  **`StageUIController.cs`**    
    -   인게임 UI 요소 전체를 총괄하는 메인 컨트롤러(Controller) 클래스입니다. `GameManager`, `WaveManager` 등 게임 로직 시스템으로부터 이벤트를 받아(`OnHpChanged`, `OnWaveIdxChanged` 등) `StageHUDView`와 같은 하위 View들에게 데이터를 전달하고 업데이트를 지시하는 중재자 역할을 수행합니다.
        
-   **`StageHUDView.cs`**    
    -   플레이 중 항상 표시되는 핵심 정보(체력, 웨이브, 재화)를 담는 HUD의 뷰(View) 클래스입니다. `StageUIController`로부터 받은 데이터를 UI 텍스트에 갱신하는 역할을 담당합니다.
        
-   **`StagePauseView.cs`**    
    -   게임 일시정지 메뉴를 담당하는 뷰(View) 클래스입니다. 메뉴를 표시할 때 `Time.timeScale`을 0으로 만들어 게임을 멈추고, '이어하기', '로비로 가기' 등의 버튼 이벤트를 처리합니다.
        
-   **`StageResultView.cs`**    
    -   스테이지 클리어 또는 실패 시 나타나는 결과 창을 담당하는 뷰(View) 클래스입니다. 성공/실패 여부에 따라 다른 UI(텍스트, 별점, 배경 색상)를 표시하고, 보상 정보를 보여주며, '재시도' 또는 '로비로 가기' 버튼 기능을 제공합니다.

---

### 🔸 **Stage/TowerBuild**

> 타워를 건설하고 배치하는 과정과 관련된 UI 및 컨트롤러를 관리합니다.

-   **`TowerPlacementController.cs`**
    
    -   타워 배치 모드의 전체 흐름을 제어하는 **싱글톤(Singleton) 컨트롤러** 클래스입니다. 타워 배치와 관련된 모든 핵심 로직을 담당합니다.
        
    -   **주요 기능:**        
        -   **입력 및 레이캐스팅**: 마우스/터치 입력을 받아 월드 좌표로 변환하고, 타워 프리뷰가 지형을 따라다니도록 합니다.            
        -   **프리뷰 제어 및 유효성 검사**: 프리뷰의 부드러운 이동과 그리드/앵커 스냅을 구현합니다. 배치 가능한 영역(`buildableMask`)인지, 장애물(`blockedMask`)과 겹치지 않는지 실시간으로 검사합니다.            
        -   **시각적 피드백**: 배치 가능 여부에 따라 프리뷰와 사거리 표시기(`RangeIndicator`)의 색상을 변경합니다. 이 때 **`MaterialPropertyBlock`** 을 사용하여 머티리얼 인스턴스를 새로 생성하지 않아 렌더링 성능을 최적화합니다.            
        -   **엣지 팬(Edge Pan)**: 프리뷰가 화면 가장자리에 닿으면 `CameraController`에 카메라 이동을 요청하여 맵을 넓게 볼 수 있도록 돕습니다.            
-   **`RangeIndicator.cs`**    
    -   타워의 공격 범위를 시각적으로 표시하는 **재사용 가능한 원형 표시기** 컴포넌트입니다.        
    -   `LineRenderer`를 사용하여 월드 공간에 원을 그리며, `TowerPlacementController`에 의해 색상과 반지름이 실시간으로 제어됩니다.

---

### 🔸 **Stage/TowerSkillView**

> 화면 좌측 하단의 타워/스킬 목록, 그리고 타워/스킬 선택 시 나타나는 상세 정보 UI를 관리합니다. 또한 월드에 배치된 타워를 선택하고 관리하는 기능도 포함합니다.

-   **`TowerSkillSelectionController.cs`**    
    -   타워/스킬 **리스트 UI**와 **상세 정보 UI**를 연결하는 **컨트롤러**입니다. 리스트에서 항목을 선택하면(`OnTowerSelected`), 해당 데이터를 상세 정보 View(`TowerInfoView`)에 전달하여 표시하고, 상세 정보 View의 구매/업그레이드 버튼 클릭 이벤트를 받아 `TowerPlacementController`나 `GameManager`에 로직 수행을 요청하는 중재자 역할을 합니다.
        
-   **`TowerWorldSelectionController.cs`**    
    -   **월드에 배치된 타워**를 클릭하여 선택하고, 해당 타워의 강화/판매 UI(`TowerInfoView`)를 표시하는 **컨트롤러**입니다. UI 클릭과 월드 클릭을 구분하며, 선택된 타워의 업그레이드 및 판매 로직을 직접 처리합니다.
        
-   **`TowerSkillListView.cs`**    
    -   화면 좌측 하단에 표시되는 타워/스킬 스크롤 목록의 **메인 뷰**입니다. `TowerListItem`, `SkillListItem` 프리팹을 동적으로 생성하며, 아이템 선택 시 `OnTowerSelected`, `OnSkillSelected` 이벤트를 발생시켜 Controller에게 알립니다.
        
-   **`TowerInfoView.cs`**    
    -   타워의 상세 정보를 표시하는 **뷰**입니다. '구매' 모드와 '강화/판매' 모드를 모두 가지고 있으며, 각 상황에 맞는 데이터를 표시하고 버튼 클릭 시 이벤트를 통해 Controller에게 알립니다.
        
-   **`SkillInfoView.cs`**    
    -   스킬의 상세 정보를 표시하는 **뷰**입니다. 현재 레벨과 다음 레벨의 스탯을 비교하여 보여주며, 스킬 실행 및 업그레이드 버튼 기능을 제공합니다.
        
-   **`TowerListItem.cs`**    
    -   `TowerSkillListView`에 사용되는 개별 타워 아이템 **뷰 컴포넌트**입니다.
        
-   **`SkillListItem.cs`**    
    -   `TowerSkillListView`에 사용되는 개별 스킬 아이템 **뷰 컴포넌트**입니다. 스킬의 쿨타임을 실시간으로 반영하는 UI 로직을 포함하고 있습니다.