
# 📂 Wave 폴더 구조 및 클래스 간략 설명

### 📌 개요

웨이브 시스템은 씬에 배치된 몬스터를 순차적으로 스폰하고, 지정된 경로(Path)를 따라 이동시키는 기능을 담당합니다.  
WaveDataSO → WaveInfo/SpawnEntry → WaveManager → MonsterMovement → WayPath/Waypoint 구조로 구성되어 있습니다.

---

## 📁 폴더 구조 및 역할

### 🔹 **Controller**

> 웨이브 진행과 몬스터 이동 로직을 구현하는 폴더입니다.

* **`WaveManager.cs`**  
  * `WaveDataSO`를 순차 로드 → `SpawnWave` 코루틴 실행 → 다음 웨이브로 자동 전환  
* **`MonsterMovement.cs`**  
  * `Waypoint[]` 경로를 순회하며 몬스터 이동 및 애니메이션 재생 

---

### 🔹 **Data**

> 웨이브 관련 ScriptableObject 및 런타임에 사용될 웨이브 데이터를 분리해 정의한 폴더입니다.

* **`WaveDataSO.cs`**  
  * 전체 웨이브 시퀀스(1~N)을 담는 ScriptableObject.  
* **`WaveInfo.cs`**  
  * 한 웨이브의 번호, SpawnEntry 배열, 다음 웨이브 대기시간을 정의.  
* **`SpawnEntry.cs`**  
  * 몬스터 데이터, 크기, 스폰 개수·간격 등 개별 엔트리 설정.

---

### 🔹 **Path**

> 몬스터가 따라갈 웨이포인트 경로를 정의하는 폴더입니다.

* **`Waypoint.cs`**  
  * 씬에 배치되는 단일 지점 컴포넌트. 다음 `Waypoint` 오브젝트를 `next` 필드로 연결.  
* **`WayPath.cs`**  
  * 첫 `Waypoint`부터 `next` 체인을 따라 배열로 변환해 제공.

---

### 🔹 **ScriptableObject**

> `Data/WaveDataSO.asset` 등 에디터에서 생성된 SO 에셋이 위치하는 폴더입니다.

* **`Stage1WaveDataSO.asset`**  
  * 몬스터 종류, 대기 시간, 각 웨이브 구성 요소 등 스테이지 1의 웨이브 정보가 담긴 ScriptableObject 에셋.

---
