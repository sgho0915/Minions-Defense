
# 📂 Towers 폴더 구조 및 클래스 간략 설명

### 📌 개요

유니티 타워디펜스 게임의 타워 관련 로직 및 데이터를 관리하는 폴더입니다.

---

## 📁 폴더 구조 및 역할

### 🔹 **Controller**

> 인터페이스를 실제 구현하여 타워의 행동을 처리하는 폴더입니다.

* **`TowerController.cs`**
  * `ITower`를 구현해 공격, 업그레이드, 판매 등의 핵심 로직을 구현한 클래스.
* **`MainTowerController.cs`**
  * 메인타워만을 위한 로직을 구현한 클래스로, 메인타워의 남은 체력 업데이트 시 이벤트 발생으로 UI 업데이트나 게임상태 업데이트를 요청.
* **`BallisticProjectile.cs`**
  * 발사 시점의 목표 위치를 향해 포물선 궤도로 날아가고, 충돌 시 데미지 전달, 효과 재생 후 자신을 파괴하는 로직을 구현한 클래스.

---

### 🔹 **Data**

> 순수 데이터 클래스와 ScriptableObject(SO)만 모아둔 폴더입니다.

* **`TowerDataSO.cs`**
  * 타워 데이터를 정의한 ScriptableObject 클래스.
* **`TowerLevelData.cs`**
  * 타워 레벨별 능력치와 설정을 담은 데이터 클래스.
* **`TowerEnums.cs`**
  * 타워 종류, 타워의 광역 공격(AoE) 형태, 타워가 공격할 타겟 우선순위를 정의하는 열거형.

---

### 🔹 **Factory**

> 팩토리 패턴을 사용하여 타워 생성 로직을 일관되게 제공합니다.

* **`TowerFactory.cs`**
  * `TowerDataSO` 기반으로 `TowerPlacementController` 로 부터 호출돼 타워 타입에 따라 적절한 타워를 생성하는 싱글톤 팩토리 클래스.

---

### 🔹 **Interface**

> 타워의 공통 동작 인터페이스와 각 모듈 간 계약을 정의한 폴더입니다.

* **`ITower.cs`**
  * 공격, 업그레이드, 판매 등 타워 공통 동작을 정의한 인터페이스.

---

### 🔹 **ScriptableObject**

> ScriptableObject의 타워 관련 `.asset` 파일들이 모이는 폴더입니다.

* **`CanonTower.asset`**
  * 캐논타워의 데이터가 담긴 ScriptableObject 에셋.
* **`MachineGunTower.asset (현재 미구현 상태)`**
  * 머신건타워의 데이터가 담긴 ScriptableObject 에셋.
* **`MageTower.asset (현재 미구현 상태)`**
  * 메이지타워의 데이터가 담긴 ScriptableObject 에셋.
* **`MainTower.asset`**
  * 메인타워의 데이터가 담긴 ScriptableObject 에셋.

---

### 🔹 **View**

> MVC 패턴 중 UI(View)를 담당하는 폴더입니다.

* **`MainTowerView.asset`**
  * `MainTowerController`로 부터 메인타워의 남은 체력 데이터 이벤트를 구독해 슬라이더로 남은 체력 UI를 업데이트 하는 클래스.


---
