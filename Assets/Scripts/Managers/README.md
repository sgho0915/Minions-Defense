
# 📂 Managers 폴더 구조 및 클래스 간략 설명

### 📌 개요

게임의 전반적인 흐름과 상태, 핵심 시스템을 관리하는 매니저(Manager) 클래스들을 모아놓은 폴더입니다.
이 클래스들은 주로 싱글톤(Singleton) 디자인 패턴을 사용하여 프로젝트 어디에서든 쉽게 접근할 수 있도록 설계되었으며, 게임의 핵심 로직을 중앙에서 통제하는 역할을 담당합니다.

---

## 📁 폴더 내 스크립트 역할

* **`GameManager.cs`**  
  * **주요 책임**
    * 게임의 전반적인 흐름과 상태를 총괄하는 싱글톤(Singleton) 관리자 클래스입니다. 씬이 변경되어도 파괴되지 않으며(DontDestroyOnLoad), 스테이지의 시작, 클리어, 실패 등 핵심 로직을 처리합니다.
    * `SceneManager.sceneLoaded` 이벤트를 구독하여 "Stage_"로 시작하는 씬이 로드될 때마다 필요한 초기화(`InitializeStageScene`) 및 다른 씬으로 전환 시 정리(`CleanupStageScene`) 작업을 자동으로 수행합니다.
    * `RunStage` 코루틴을 통해 웨이브 시작, 모든 몬스터 처치 여부 확인 등 스테이지의 전체적인 흐름을 제어하고, 결과에 따라 스테이지 클리어(`HandleStageClear`) 또는 실패(`HandleStageFail`) 처리를 담당합니다.
    * 스테이지 내에서 사용하는 임시 재화(`stagePoints`)와 영구적으로 저장되는 계정 재화(`globalPoints`)를 관리합니다. PlayerPrefs를 통해 게임 재화와 스테이지 클리어 상태(별 개수)를 저장하고 로드합니다.
    * 스테이지가 시작되면 `WaveManager`, `MainTowerController`, `StageUIManager` 등 씬에 존재하는 핵심 컴포넌트들을 찾아 참조를 설정합니다. 이들과 event를 통해 게임의 주요 상황 변화를 감지하고 대응하는 역할을 합니다.
   * **디자인 패턴**
     * **싱글톤 패턴**: Instance 프로퍼티를 통해 게임 내 어디서든 유일한 `GameManager` 객체에 접근할 수 있도록 하여, 게임 상태와 핵심 로직을 일관성 있게 관리합니다.
     * **단일 책임 원칙**: `GameManager`는 '게임의 흐름과 상태 관리'라는 단일 책임을 가집니다. 몬스터 생성(`WaveManager`), UI 표시(`UIManager`), 타워 로직(`TowerController`) 등은 다른 객체에 위임하고, 자신은 이들을 총괄하는 지휘자(Controller)의 역할에 집중하여 코드의 유지보수성과 확장성을 높입니다.
     * **옵저버 패턴**: `OnStagePointsChanged`와 같은 이벤트를 제공하고, `mainTower.OnDied` 등의 이벤트를 구독함으로 다른 객체와 낮은 의존성과 결합도를 유지합니다. 이는 각 객체가 서로를 직접 알지 못해도 상호작용할 수 있게 하여 시스템을 유연하게 만듭니다.
  
---