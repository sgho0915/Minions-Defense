
# 📂 UserSkills 폴더 구조 및 클래스 간략 설명

### 📌 개요

유니티 타워디펜스 게임의 **사용자 스킬(User Skills)** 시스템을 관리하는 폴더입니다.  
공통 레벨 데이터 → ScriptableObject → 팩토리 → 컨트롤러 → 투사체/트랩 로직으로 이어지는 일관된 구조를 갖추고 있습니다.

---

## 📁 폴더 구조 및 역할

### 🔹 **Controllers**

> 인터페이스를 구현해 실제 스킬 로직을 구현하는 폴더입니다.

* **`MagicPoeController.cs`**  
  * `ISkill`을 상속받아 마법의 포댕이 스킬에 대한 초기화, 시전, 비용, 사용 규칙 등의 조건을 관리하는 컨트롤러 클래스
* **`MagicPoePatroller.cs`**  
  * MagicPoeLevelData, WayPoint 정보를 받아 마법의 포댕이 프리팹 인스턴스가 정해진 경로를 따라 이동, 공격, 소멸하는 유닛 자체의 행동을 관리하는 컨트롤러 클래스

---

### 🔹 **Data**

> 스킬에 대한 공통 정보와 공통 정보를 가진 개별 스킬에 대한 레벨별 데이터와 관리 로직이 담긴 폴더입니다.

* **`MagicPoeDataSO.cs`**  
  * `SkillDataSO` 를 상속받아 마법의 포댕이 스킬 컨트롤러를 생성하고 초기화 하는 ScriptableObject
* **`MagicPoeLevelData.cs`**  
  * `SkillLevelData` 를 상속받은 마법의 포댕이 스킬 레벨별 데이터
* **`SkillDataSO.cs`**  
  * 모든 스킬 데이터 SO의 기반이 되는 추상클래스로 UI 표시에 필요한 공통 정보, 자신을 제어할 Controller를 생성하는 책임
* **`SkillLevelData.cs`**  
  * 모든 스킬 데이터의 기반이 되는 추상클래스로 UI 표시 및 공통 로직에 필요한 최소한의 데이터를 정의
  
---

### 🔹 **Interface**

> 스킬 컨트롤러가 반드시 구현해야 할 계약(인터페이스)을 정의한 폴더입니다.

* **`ISkill.cs`**  
  * `Initialize(dataSO, owner)`, `SetLevel(level)`, `CastSkill(targetPos)`, `IsReady()` 등 스킬 동작에 대한 인터페이스로, 데이터와 주체로 초기화 해 어떤 스킬 데이터 SO든 받을 수 있도록 정의

---

### 🔹 **ScriptableObject**

> ScriptableObject의 스 관련 `.asset` 파일들이 모이는 폴더입니다.

* **`MagicPoeDataSO.cs`**  
  * “마법의 포댕이” 전체 레벨(1~3) 설정 데이터가 담긴 ScriptableObject 에셋.

---
