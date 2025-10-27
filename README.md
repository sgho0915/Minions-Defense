




# Minions Defense
ScriptableObject 기반 데이터 설계와 디자인 패턴을 적용해 확장성과 유지보수성을 목표로 개발한 Unity 3D 타워 디펜스 게임입니다.

---


## 📚 목차
1.  [**게임 플레이 스크린샷 (Screenshot)**](#screenshot)                                   
2.  [**데모 영상 (Play Video)**](#play-video)                                              
3.  [**게임 플레이 데모 (Play Demo)**](#play-demo)                                         
4.  [**다운로드 (Download)**](#download)                                                   
5.  [**프로젝트 개요 (Overview)**](#overview)                                             
6.  [**기술 스택 (Tech Stack)**](#tech-stack)                                              
7.  [**브랜치 전략 (Git Flow)**](#git-flow)                                                
8.  [**핵심 시스템 설계 및 구현**](#core-systems)                                         
	   -  [데이터 기반 설계 (ScriptableObject)](#so-architecture)                             
	   -  [확장 가능한 스킬 시스템 (Skill System)](#skill-system)                             
	   -  [이벤트 기반 UI 시스템 (UI System)](#ui-system)                                      
	   -  [타워 라이프사이클 관리](#tower-lifecycle)                                         
9.  [**프로젝트 구조 (Project Structure)**](#project-structure)                         
10. [**개발 과정 및 회고**](#retrospective)                                               


---

<a id="screenshot"></a>
## 📸 1. 게임 플레이 스크린샷 (Screenshot)
![Gameplay Screenshot](https://github.com/user-attachments/assets/996a715a-9ae8-4b7b-bd84-d20ab137cc70)

---

<a id="play-video"></a>
 ## 🎬 2. 데모 영상 (Play Video)
[![데모 영상](https://utfs.io/f/nGnSqDveMsqxAyFASoX1nkORdXhc4E0vWDI9uNrjbgp7xoKV)](https://www.youtube.com/watch?v=d9tJlxZBqEY)

---

<a id="play-demo"></a>
## 🎮 3. 게임 플레이 데모 (Play Demo)

> ### **[► 데모 플레이 (GitHub Pages)](https://sgho0915.github.io/Minions-Defense/WebBuildTest/)**

---

<a id="download"></a>
## 🚀 4. 다운로드 (Download)


<!-- 이 프로젝트는 현재 Google Play Store에서 다운로드할 수 있습니다.

### ⬇️ Google Play

<a href="https://play.google.com/store/apps/details?id=com.SeongHo.TowerDefensePortfolio"> <img src="https://play.google.com/intl/ko_kr/badges/images/generic/ko_badge_web_generic.png" alt="Google Play에서 다운로드하세요" height="60"/> </a> -->

### 🔗 GitHub Release (AAB/APK 파일)
> 직접 테스트를 위해 빌드 파일을 다운로드할 수 있습니다. [**Release 페이지 바로가기**](https://github.com/YourRepo/releases)

---

<a id="overview"></a>
## 📌 5. 프로젝트 개요 (Overview)

**Minions Defense**는 확장성과 유지보수성을 고려한 시스템 설계를 목표로 개발한 3D 타워 디펜스 게임입니다.
단순히 필요한 기능들을 구현하기 보다 근본적으로 좀 더 유연하고 재사용 가능한 코드를 작성하는 것을 목표로 ScriptableObject를 활용한 게임 속 데이터 관리, 클래스 간 책임 분리, 상황에 맞는 디자인 패턴을 프로젝트에 적용하며 문제 해결 능력을 기르기 위해 진행되었습니다.

---

<a id="tech-stack"></a>
## 🛠 6. 기술 스택 (Tech Stack)
-   **Engine**: <span style="vertical-align: middle;">![Unity](https://img.shields.io/badge/Unity-6000.2.5f1-red)</span>
-   **Language**: `C#`
-   **Version Control**: `GitHub`
-   **Project Management**: `Git-Flow` 브랜칭, 개발 브랜치별 PR 관리

---

<a id="git-flow"></a>
## 🤝 7. 브랜치 전략 (Git Flow)

-   **`main`**
    -   배포 전용 브랜치
    -   태그된 릴리즈만 존재
-   **`develop`**
    -   통합 개발 브랜치
    -   모든 기능이 이곳으로 merge
-   **`feature/<기능명>`**
    -   개별 기능 개발용 브랜치
    -   `develop` → 분기 → 기능 구현 → `develop`에 머지 → 삭제
-   **`release/vX.Y.Z`**
    -   릴리즈 준비용 브랜치
    -   `develop` → 분기 → 버전·문서 → `main`·`develop`에 머지 → 삭제
-   **`hotfix/vX.Y.Z`**
    -   긴급 버그 수정용 브랜치
    -   `main` → 분기 → 수정 → `main`·`develop`에 머지 → 삭제

---

<a id="core-systems"></a>
## 🔩 8. 핵심 시스템 설계 및 구현

<a id="so-architecture"></a>
### 1. 데이터 기반 설계 (ScriptableObject)
게임의 핵심 요소인 타워, 몬스터, 스킬, 웨이브 데이터를 모두 ScriptableObject로 관리하여 기획 데이터와 로직을 분리했습니다. 이를 통해 코드 수정 없이 밸런싱 및 콘텐츠 확장이 가능한 유연한 구조를 목표로 했습니다.

<a id="skill-system"></a>
### 2. 확장 가능한 스킬 시스템
`ISkill` 인터페이스를 중심으로 전략(Strategy) 및 팩토리 메서드(Factory Method) 패턴을 적용하여, 새로운 스킬이 추가되더라도 기존 시스템의 코드를 수정할 필요가 없는 **개방 폐쇄 원칙(OCP)**을 준수하는 구조로 설계했습니다.
-   **`ISkill`**: 스킬의 실행, 쿨다운 등 공통 규약을 정의한 인터페이스
-   **`SkillDataSO`**: 팩토리 메서드 `CreateSkill()`을 통해 자신에게 맞는 스킬 컨트롤러를 생성
-   **`MagicPoeController`**: `ISkill`을 구현한 구체적인 스킬 로직

<a id="ui-system"></a>
### 3. 이벤트 기반 UI 시스템
**옵저버(Observer)** 패턴을 활용하여 게임 로직과 UI 시스템 간의 의존성을 제거했습니다. 예를 들어, `GameManager`는 몬스터 처치 시 `OnStagePointsChanged` 이벤트를 발행하기만 하면 되고, UI는 이 이벤트를 구독하여 스스로 텍스트를 갱신합니다. 이를 통해 각 시스템이 독립적으로 동작하고 테스트할 수 있도록 구현했습니다.

<a id="tower-lifecycle"></a>
### 4. 타워 라이프사이클 관리
타워의 구매, 배치, 선택, 강화, 판매로 이어지는 전체 흐름을 구현했습니다. 특히, 타워의 선택과 배치를 담당하는 책임을 `TowerPlacementController`와 `TowerWorldSelectionController`로 분리하여 단일 책임 원칙을 준수하고자 노력했습니다.

---

<a id="project-structure"></a>
## 📂 9. 프로젝트 구조 (Project Structure)
프로젝트는 기능 단위의 폴더 구조를 따르며, 각 폴더에는 해당 시스템의 설계 의도와 클래스 역할을 설명하는 `README.md` 파일이 포함되어 있습니다.

-   **`Assets/Scripts/`**: C# 스크립트
    -   **[`Managers/`](https://github.com/sgho0915/Minions-Defense/blob/develop/Assets/Scripts/Managers/README.md)** : 게임의 핵심 로직 (GameManager, SettingManager 등)
    -   **[`Monsters/`](https://github.com/sgho0915/Minions-Defense/blob/develop/Assets/Scripts/Monsters/README.md)** : 몬스터 관련 로직 및 데이터 관리
    -   **[`Towers/`](https://github.com/sgho0915/Minions-Defense/blob/develop/Assets/Scripts/Towers/README.md)** : 타워 시스템 관련 로직 및 데이터 관리
    -   **[`UI/`](https://github.com/sgho0915/Minions-Defense/blob/develop/Assets/Scripts/UI/README.md)** : UI 컨트롤러 및 View 스크립트
	    - **Lobby/** : 메인로비, 스테이지 선택 등 스테이지 시작 전 사용되는 UI 관리
	    - **Stage/** : 인게임 플레이 중 표시되는 UI(HUD, 일시정지, 결과 창) 관리
		    - **TowerBuild/** : 타워 건설 및 배치 관련 UI 및 컨트롤러 관리
		    - **TowerSkillView/** : 타워 및 스킬 목록, 선택 시 표시되는 상세 정보 UI 관리
    -   **[`UserSkills/`](https://github.com/sgho0915/Minions-Defense/blob/develop/Assets/Scripts/UserSkills/README.md)**: 스킬 시스템 관련 로직 및 데이터 관리
    -   **[`Utils/`](https://github.com/sgho0915/Minions-Defense/blob/develop/Assets/Scripts/Utils/README.md)**: 게임 전반에 사용되는 유틸리티 스크립트 관리
    -   **[`Waves/`](https://github.com/sgho0915/Minions-Defense/blob/develop/Assets/Scripts/Waves/README.md)**: 웨이브, 몬스터 이동 경로 관련 로직 및 데이터 관리

---

<a id="retrospective"></a>
## 📈 10. 개발 과정 및 회고
###  1. 프로젝트 목표
  유니티 개발자로서 한 단계 성장하기 위해 개인 포트폴리오 프로젝트로 타워 디펜스 게임을 기획했습니다. 단순히 기능을 완성하는 것보다, 그동안 실무에서 부족하다고 느꼈던 좋은 설계와 디자인 패턴, 그리고 SOLID 원칙에 기반한 클린 코드를 학습하고 직접 적용하는 과정에 가장 큰 의미를 두었습니다.
  
 ### 2. 주요 개발 내용 및 학습
 프로젝트를 진행하며 다음과 같은 기술과 설계를 적용하고 학습했습니다.
 -  **데이터 기반 설계 (ScriptableObject)**
	 - 몬스터, 타워, 스킬 등 주요 객체의 데이터를 `ScriptableObject` 로 분리하여 설계했습니다.
	 - **학습한 점:** 데이터와 로직의 분리를 명확히 하여 확장성과 유지보수성을 크게 향상시켰습니다. 기획 데이터(몬스터 체력, 타워 공격력 등) 변경 시 코드를 재컴파일할 필요 없이 SO 에셋 파일만 수정해 테스트와 디버깅이 매우 용이해지는 것을 체감했습니다.
 -  **디자인 패턴 적용**
	 - **옵저버 패턴 (Observer Pattern):** 플레이어의 재화, 몬스터의 생명력 변화 등 주요 이벤트를 `event`를 사용해 구현했습니다. UI 시스템이 각 데이터를 직접 참조하는 것이 아닌, 이벤트를 구독하도록 하여 시스템 간의 결합도를 성공적으로 낮췄습니다.
	 - **팩토리 패턴 (Factory Pattern):** 다양한 종류의 타워와 몬스터를 생성하는 로직을 별도의 팩토리 클래스로 분리했습니다. 이를 통해 객체 생성 로직의 캡슐화 및 코드의 단순화를 이룰 수 있었습니다.
	 - **싱글톤 패턴 (Singleton Pattern):** `GameManager`, `UIManager` 등 게임 전역에서 유일하게 존재해야 하는 관리자 객체에 싱글톤 패턴을 적용하여 데이터 접근의 일관성을 확보했습니다.
-   **3D 환경 프로그래밍 및 물리**    
    -   부족하다고 느꼈던 3D 환경에서의 프로그래밍, 특히 `Raycast`를 활용한 타워 배치 시스템(배치 가능/불가능 영역 구분), `Quaternion`과 `Vector` 연산을 통한 투사체 발사 및 포탑의 부드러운 회전 로직을 구현하며 3D 수학과 유니티 물리 엔진에 대한 이해도를 높이려 노력했습니다. 하지만 여전히 3D 환경 프로그래밍에 대해 많은 부족함을 느끼고 있어 추후 도서 구입과 새로운 프로젝트를 통해 추가적인 학습과 사용 경험이 필요함을 느꼈습니다.

### 3. 아쉬운 점 및 개선 과제
목표했던 바를 일부 달성했지만, 시간적 한계와 경험 부족으로 다음과 같은 아쉬움이 남았습니다.
-  **성능 최적화의 부재 (Object Pooling & Addressable)**
	- 가장 아쉬운 점은 리소스 관리의 중요성을 간과한 것입니다. SpriteAtlas와 같은 소소한 최적화 요소는 적용해 배치가 줄어드는 것은 확인했지만, 몬스터와 투사체처럼 빈번하게 생성되고 파괴되는 오브젝트에 대해 오브젝트 풀링(Object Pooling)을 적용하지 못했습니다. 이로 인해 불필요한 `Instantiate`와 `Destroy` 호출이 잦아져 가비지 컬렉션(GC) 부담과 성능 저하를 유발할 수 있습니다.
	- 또한, 모든 리소스를 씬에 포함시켜 빌드했습니다. 프로젝트 규모가 커질 경우를 대비해 어드레서블 애셋 시스템(Addressable Asset System)을 도입하여 메모리 관리 효율을 높이고, 런타임에 리소스를 유연하게 로드/언로드하는 방식을 학습하지 못한 점이 큰 아쉬움으로 남습니다. 이는 향후 리팩토링이나 타 프로젝트 진 시 최우선으로 적용할 과제입니다.
-  **견고하지 못한 로직 및 테스트 부재**
	- 목표했던 개발 기간 대비 타워 배치 로직의 디테일을 구현하는 데 예상보다 많은 시간이 소요되었습니다. 이로 인해 단위 테스트 작성을 소홀히 하여, 로직 변경 시 발생할 수 있는 사이드 이펙트를 사전에 방지하지 못했습니다.
	- 스킬 및 UI 관련 구조가 후반부로 갈수록 복잡해져, 추가적인 리팩토링과 구조 개선이 필요함을 느꼈습니다.

### 4. 형상관리에 대한 회고
- 1인 프로젝트임에도 불구하고 체계적인 형상관리 경험을 위해 Git Flow 브랜칭 전략을 도입했습니다.
- **장점:** `main`(배포), `develop`(개발), `feature`(기능) 브랜치를 명확히 분리해 기능 개발과 안정적인 버전 관리를 동시에 추구할 수 있었습니다. 특히 `feature` 브랜치에서 개별 기능을 독립적으로 개발하고 충분한 테스트 후 `develop`에 병합하는 과정은 비록 1인 프로젝트였지만 코드 리뷰와 협업의 좋은 연습이 되었습니다.
- **단점:** 반면, 소규모 1인 프로젝트에서 Git Flow는 다소 과한 측면이 있었습니다. 브랜치를 생성하고 병합하는 과정이 비교적 복잡했고, feature 브랜치에서 외부 에셋을 gitignore에 추가해 추적에서 제외시켰다가 잊어버린 후 develop 브랜치로 병합 후 프로젝트가 꼬여 체리 픽으로 특정 파일만 복구하거나 에셋을 다시 임포트 해야하는 경우도 있었고, 간단한 기능 수정에도 시간이 소요되었습니다. 다음 프로젝트에서는 규모에 따라 `main`과 `feature` 브랜치만 사용하는 더 가벼운 방법 적용하는 것도 고려해 볼 것입니다.

### 5. 레벨 디자인 경험
- 프로그래밍 외에 레벨 디자인을 직접 경험한 것도 큰 수확입니다. 몬스터의 이동 경로를 설계하고, 웨이브 별 몬스터의 수량, 체력, 속도, 보상을 조절하며 게임의 난이도 곡선을 직접 기획했습니다.
- 또한, 좋은 게임을 만들기 위해서는 리소스나 개발에 대한 역량도 당연히 중요하지만 UI나 타워, 몬스터, 스킬 같이 게임을 구성하는 요소들에 대한 설계 기반에 많은 노력과 고민을 기울일수록 좋은 게임이 만들어진다는 것을 몸소 깨닫게 되었습니다.

### 6. 결론 및 향후 계획
이번 프로젝트는 '완성'보다 '성장'에 목표를 둔 만큼, SOLID 원칙과 디자인 패턴을 고민하며 클린 코드를 작성하려 노력한 과정 자체로 성공적이었다고 생각합니다.
동시에 오브젝트 풀링과 어드레서블 시스템의 부재가 성능에 미치는 영향, 그리고 3D 요소를 다루는 부분에서의 부족함을 명확히 인지하는 계기가 되었습니다. 향후 이 프로젝트를 리팩토링하며 아쉬웠던 점들을 보완하고, 성능 최적화와 더욱 견고한 설계에 더 많은 노력을 기울일 것입니다.
