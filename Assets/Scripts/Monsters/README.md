
# 📂 Monsters 폴더 구조 및 클래스 간략 설명

### 📌 개요

유니티 타워디펜스 게임의 몬스터 관련 로직 및 데이터를 관리하는 폴더입니다.

---

## 📁 폴더 구조 및 역할

### 🔹 **Controller**

> MVC 패턴 중 로직(Controller)을 담당하는 폴더입니다.

* **`MonsterController.cs`**  
  * `IMonster`의 구현체, 이동·공격·사망·보상 로직과 Model·View 연동을 수행하는 클래스
* **`MonsterProjectile.cs`**  
  * `MonsterLevelData`의 정보를 가지고 몬스터의 투사체 속도, 데미지 정보를 가져와 충돌체의 Tag 체크 후 데미지를 입히는 클래스
---

### 🔹 **Data**

> 순수 데이터 클래스와 ScriptableObject(SO)만 모아둔 폴더입니다.

* **`MonsterDataSO.cs`**  
  * 몬스터 기본 데이터를 정의한 ScriptableObject 클래스
* **`MonsterLevelData.cs`**  
  * 몬스터 레벨별 기본 정보, 스탯, 프리팹, 보상 정보를 담은 데이터 클래스
* **`MonsterEnum.cs`**  
  * 몬스터 종류, 크기(레벨)를 구분하는 열거형
  
---

### 🔹 **Factory**

> 팩토리 패턴을 사용하여 몬스터 생성 로직을 일관되게 제공합니다.

* **`MonsterFactory.cs`**  
  * `MonsterDataSO` 기반으로 웨이브 시작 시 `WaveManager`로부터 호출돼 요청받은 크기·타입의 몬스터를 생성하는 싱글톤 팩토리 클래스.

---

### 🔹 **Interface**

> 몬스터의 공통 동작 인터페이스와 각 모듈 간 계약을 정의한 폴더입니다.

* **`IMonster.cs`**  
  * 몬스터 초기화, 크기 설정, 보상 등 몬스터 공통 동작을 정의한 인터페이스

---

### 🔹 **Model**

> MVC 패턴 중 데이터(Model)를 담당하는 폴더입니다.

* **`MonsterModel.cs`**  
  * 현재, 최대 체력 정보 관리 및 피격 시 체력 변화, 몬스터 사망 이벤트를 발생하는 클래스.

---

### 🔹 **ScriptableObject**

> ScriptableObject의 몬스터 관련 `.asset` 파일들이 모여있는 폴더입니다.

* **`CapitalistDataSO.asset (현재 미구현 상태)`**  
  * 자본주의 괴물 몬스터의 데이터가 담긴 ScriptableObejct 에셋 
* **`MoleDataSO.asset (현재 미구현 상태)`**  
  * 못난이 두더지 몬스터의 데이터가 담긴 ScriptableObejct 에셋 
* **`SkeletonDataSO.asset`**  
  * 스켈레톤 몬스터의 데이터가 담긴 ScriptableObejct 에셋 
* **`SpiritDataSO.asset`**  
  * 유령 몬스터의 데이터가 담긴 ScriptableObejct 에셋 
  
---

### 🔹 **View**

> MVC 패턴 중 UI(View)를 담당하는 폴더입니다.

* **`MonsterView.cs`**  
  * `MonsterModel` 이벤트를 구독하여 체력바 UI를 갱신하는 컴포넌트.

---
