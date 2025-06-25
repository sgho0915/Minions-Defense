# 📂 Towers 폴더 구조 및 클래스 간략 설명

### 📌 개요

유니티 타워디펜스 게임의 타워 관련 로직 및 데이터를 관리하는 폴더입니다.

---

## 📁 폴더 구조 및 역할

### 🔹 **Data**

> 순수 데이터 클래스와 ScriptableObject(SO)만 모아둔 폴더입니다.

* **`TowerDataSO.cs`**

  * 타워 데이터를 정의한 ScriptableObject 클래스.
* **`TowerLevelData.cs`**

  * 타워 레벨별 능력치와 설정을 담은 데이터 클래스.

---

### 🔹 **Interfaces**

> 타워의 공통 동작 인터페이스와 각 모듈 간 계약을 정의한 폴더입니다.

* **`ITower.cs`**

  * 공격, 업그레이드, 판매 등 타워 공통 동작을 정의한 인터페이스.

---

### 🔹 **Factory**

> 팩토리 패턴을 사용하여 타워 생성 로직을 일관되게 제공합니다.

* **`TowerFactory.cs`**

  * 각 타워 타입에 따라 적절한 타워를 생성하는 팩토리 클래스.

---

### 🔹 **Controllers**

> 인터페이스를 실제 구현하여 타워의 행동을 처리하는 폴더입니다.

* **`TowerController.cs`**

  * 공격, 업그레이드, 판매 등의 핵심 로직을 구현한 클래스.

---

### 🔹 **Enums**

> 프로젝트 전반에서 사용되는 열거형(enum)들을 정의한 폴더입니다.

* **`TowerType.cs`**

  * 타워 종류를 구분하는 열거형.
* **`TowerLevel4Type.cs`**

  * 타워 4단계 강화 시 A/B 타입 분기를 정의하는 열거형.
* **`TowerAreaShape.cs`**

  * 타워의 광역 공격(AoE) 형태를 정의하는 열거형.
* **`TowerTargetPriority.cs`**

  * 타워가 공격할 타겟 우선순위를 정의하는 열거형.

---

### 🔹 **ScriptableObjects**

> 유니티 에디터에서 생성된 실제 `.asset` 파일들이 모이는 폴더입니다.

---

### 🔹 **Prefabs**

> 실제 게임에서 사용되는 프리팹을 보관하는 폴더입니다.

* 프리팹은 `TowerDataSO`와 `TowerController` 컴포넌트를 포함합니다.

---
