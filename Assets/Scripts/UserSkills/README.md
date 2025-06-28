# 📂 UserSkills 폴더 구조 및 클래스 간략 설명

### 📌 개요

유니티 타워디펜스 게임의 **사용자 스킬(User Skills)** 시스템을 관리하는 폴더입니다.  
공통 레벨 데이터 → ScriptableObject → 팩토리 → 컨트롤러 → 투사체/트랩 로직으로 이어지는 일관된 구조를 갖추고 있습니다.

---

## 📁 폴더 구조 및 역할

### 🔹 **Data**

> 레벨별 파라미터만 담는 순수 데이터 모델 클래스

* **`CommonSkillLevelData.cs`**  
  * 모든 스킬이 공유하는 공통 필드(`level`, `upgradeCost`, `modelPrefab`, `cooldown`, `skillIcon`, `skillDesc` 등)  
* **`MagicPoeLevelData.cs`**  
  * “마법의 포댕이” 전용 레벨 데이터(`moveSpeed`, `collideRadius`, `baseDamage`, `knockbackForce`, `stunDuration`, `chainRadius`, 이펙트·사운드 등)  
* **`SpikeLevelData.cs`**  
  * “스파이크” 전용 레벨 데이터(`trapDuration`, `trapRadius`, `damagePerSecond`, `slowAmount`, `slowDuration`, 이펙트·사운드 등)  

---

### 🔹 **Enums**

> 스킬 시스템 전반에서 재사용되는 열거형 정의

* **`SkillEnums.cs`**  
  * 스킬 종류 구분(`MagicPoe`, `Spike`)  

---

### 🔹 **Interface**

> 스킬 컨트롤러가 반드시 구현해야 할 계약(인터페이스)

* **`ISkill.cs`**  
  * `Initialize(dataSO, owner)`, `SetLevel(level)`, `CastSkill(targetPos)`, `IsReady()` 등 메서드 시그니처 정의  

---

### 🔹 **Factory**

> ScriptableObject → 컨트롤러 인스턴스화 역할

* **`SkillFactory.cs`**  
  * `MagicPoeDataSO` → `MagicPoeController`  
  * `SpikeDataSO` → `SpikeController`  
  * `CreateSkill(dataSO, owner)` 메서드 제공  

---

### 🔹 **Controllers**

> 실제 스킬 로직을 구현하는 컴포넌트

* **`BaseSkillController.cs`**  
  * 제네릭 공통 로직(쿨다운, 레벨 선택, 공통 필드 접근)  
* **`MagicPoeController.cs`**  
  * 고양이 투사체 생성·이동·애니메이션·`MagicPoeProjectile` 연결  
* **`MagicPoeProjectile.cs`**  
  * 투사체 충돌 시 넉백·스턴·데미지·체인 연쇄 처리  
* **`SpikeController.cs`**  
  * 지정 지점에 스파이크 트랩 설치·`SpikeTrap` 연결  
* **`SpikeTrap.cs`**  
  * 트랩 영역 진입 몬스터에 초당 데미지·감속 적용·유지시간 후 소멸  

---

### 🔹 **ScriptableObjects**

> 에디터에서 `.asset`으로 생성되는 스킬 데이터

* **`MagicPoeDataSO.cs`**  
  * “마법의 포댕이” 전체 레벨(1~3) 설정 보관  
* **`SpikeDataSO.cs`**  
  * “스파이크” 전체 레벨(1~3) 설정 보관  

---

### 🔹 **Prefabs**
  * 게임에서 사용되는 레벨별 스킬 프리팹 모음
  * 각 프리팹에는 대응 컨트롤러(`MagicPoeController`/`SpikeController`)와 이펙트용 컴포넌트가 포함