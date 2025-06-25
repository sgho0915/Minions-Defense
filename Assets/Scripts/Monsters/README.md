# 📂 Monsters 폴더 구조 및 클래스 간략 설명

### 📌 개요

유니티 타워디펜스 게임의 몬스터 관련 로직 및 데이터를 관리하는 폴더입니다.

---

## 📁 폴더 구조 및 역할

### 🔹 **Data**

> 순수 데이터 클래스와 ScriptableObject(SO)만 모아둔 폴더입니다.

* **`MonsterDataSO.cs`**  
  * 몬스터 데이터를 정의한 ScriptableObject 클래스.  
* **`MonsterLevelData.cs`**  
  * 몬스터 크기(레벨)별 스탯·애셋을 담은 데이터 클래스.

---

### 🔹 **Enums**

> 프로젝트 전반에서 사용되는 열거형(enum)들을 정의한 폴더입니다.

* **`MonsterEnum.cs`**  
  * 몬스터 종류, 크기(레벨)를 구분하는 열거형.  

---

### 🔹 **Interfaces**

> 몬스터의 공통 동작 인터페이스와 각 모듈 간 계약을 정의한 폴더입니다.

* **`IMonster.cs`**  
  * 이동, 공격, 사망·보상 등 몬스터 공통 동작을 정의한 인터페이스.

---

### 🔹 **Factory**

> 팩토리 패턴을 사용하여 몬스터 생성 로직을 일관되게 제공합니다.

* **`MonsterFactory.cs`**  
  * `MonsterDataSO` 기반으로 적절한 크기·타입의 몬스터를 생성하는 싱글톤 팩토리 클래스.

---

### 🔹 **Model**

> MVC 패턴 중 데이터(Model)를 담당하는 폴더입니다.

* **`MonsterModel.cs`**  
  * 런타임 체력 정보 및 체력 변화 이벤트를 관리하는 클래스.

---

### 🔹 **View**

> MVC 패턴 중 UI(View)를 담당하는 폴더입니다.

* **`MonsterView.cs`**  
  * `MonsterModel` 이벤트를 구독하여 체력바 UI를 갱신하는 컴포넌트.

---

### 🔹 **Controllers**

> MVC 패턴 중 로직(Controller)을 담당하는 폴더입니다.

* **`MonsterController.cs`**  
  * 이동·공격·사망·보상 로직과 Model·View 연동을 수행하는 클래스.

---

### 🔹 **ScriptableObjects**

> 유니티 에디터에서 생성된 실제 `.asset` 파일들이 모이는 폴더입니다.

---

### 🔹 **Prefabs**

> 실제 게임에서 사용되는 몬스터 프리팹을 보관하는 폴더입니다.

* 프리팹은 `MonsterDataSO`, `MonsterController`, `MonsterHealthView` 컴포넌트를 포함합니다.

---
