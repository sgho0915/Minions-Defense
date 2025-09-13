
# Minions Defense
ScriptableObject 기반 데이터 설계와 디자인 패턴을 적용해 확장성과 유지보수성을 목표로 개발한 Unity 3D 타워 디펜스 게임입니다.

---

![Gameplay Screenshot](https://blog.kakaocdn.net/dna/bCxCsv/btsPsUvIsrB/AAAAAAAAAAAAAAAAAAAAAOtopD-q7JXJLqNog1z22eje02yWnrk8RzuYlHgGotkU/img.gif?credential=yqXZFxpELC7KVnFOS48ylbz2pIh7yKj8&expires=1759244399&allow_ip=&allow_referer=&signature=tALvQQQ9a5J2x5lyy4sRV5%2BPRfk%3D)
 
[![데모 영상](https://utfs.io/f/nGnSqDveMsqx869v6NfsgcqER67P9XGtSfe5dyInlUv8us1K)](https://www.youtube.com/watch?v=QpGhH3YrmQY)

## 📚 목차
1.  [**프로젝트 개요 (Overview)**](#-프로젝트-개요)
2.  [**게임 플레이 데모 (Demo)**](#-게임-플레이-데모)
3.  [**기술 스택 (Tech Stack)**](#-기술-스택)
4.  [**브랜치 전략 (Git Flow)**](#-브랜치-전략)
5.  [**핵심 시스템 설계 및 구현**](#-핵심-시스템-설계-및-구현)
    -   [데이터 기반 설계 (ScriptableObject Architecture)](#1-데이터-기반-설계-scriptableobject-architecture)
    -   [확장 가능한 스킬 시스템 (Skill System)](#2-확장-가능한-스킬-시스템)
    -   [이벤트 기반 UI 시스템 (UI System)](#3-이벤트-기반-ui-시스템)
    -   [타워 라이프사이클 관리](#4-타워-라이프사이클-관리)
6.  [**프로젝트 구조 (Project Structure)**](#-프로젝트-구조)
7.  [**개발 과정 및 회고**](#-개발-과정-및-회고)

---

## 📌 프로젝트 개요

**Minions Defense**는 확장성과 유지보수성을 고려한 시스템 설계를 목표로 개발한 3D 타워 디펜스 게임입니다.
단순히 필요한 기능들을 구현하기 보다 근본적으로 좀 더 유연하고 재사용 가능한 코드를 작성하는 것을 목표로 ScriptableObject를 활용한 게임 속 데이터 관리, 클래스 간 책임 분리, 상황에 맞는 디자인 패턴을 프로젝트에 적용하며 문제 해결 능력을 기르기 위해 진행되었습니다.

---

## 🎮 게임 플레이 데모

> ### **[► 데모 플레이 (GitHub Pages)](https://sgho0915.github.io/Minions-Defense/WebBuildTest/)**

---

## 🛠 기술 스택
-   **Engine**: ![Unity](https://img.shields.io/badge/Unity-6000.2.2f1-blue)
-   **Language**: `C#`
-   **Version Control**: `GitHub`
-   **Project Management**: `Git-Flow` 브랜칭 전략, 개발 브랜치별 PR 관리

---
## 🤝 브랜치 전략

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

## 🔩 핵심 시스템 설계 및 구현

### 1. 데이터 기반 설계 (ScriptableObject Architecture)
게임의 핵심 요소인 타워, 몬스터, 스킬, 웨이브 데이터를 모두 ScriptableObject로 관리하여 기획 데이터와 로직을 분리했습니다. 이를 통해 코드 수정 없이 밸런싱 및 콘텐츠 확장이 가능한 유연한 구조를 목표로 했습니다.

### 2. 확장 가능한 스킬 시스템
`ISkill` 인터페이스를 중심으로 **전략(Strategy)** 및 **팩토리 메서드(Factory Method)** 패턴을 적용하여, 새로운 스킬이 추가되더라도 기존 시스템의 코드를 수정할 필요가 없는 **개방 폐쇄 원칙(OCP)**을 준수하는 구조로 설계했습니다.
-   **`ISkill`**: 스킬의 실행, 쿨다운 등 공통 규약을 정의한 인터페이스
-   **`SkillDataSO`**: 팩토리 메서드 `CreateSkill()`을 통해 자신에게 맞는 스킬 컨트롤러를 생성
-   **`MagicPoeController`**: `ISkill`을 구현한 구체적인 스킬 로직

### 3. 이벤트 기반 UI 시스템
**옵저버(Observer)** 패턴을 활용하여 게임 로직과 UI 시스템 간의 의존성을 제거했습니다. 예를 들어, `GameManager`는 몬스터 처치 시 `OnStagePointsChanged` 이벤트를 발행하기만 하면 되고, UI는 이 이벤트를 구독하여 스스로 텍스트를 갱신합니다. 이를 통해 각 시스템이 독립적으로 동작하고 테스트할 수 있도록 구현했습니다.

### 4. 타워 라이프사이클 관리
타워의 구매, 배치, 선택, 강화, 판매로 이어지는 전체 흐름을 구현했습니다. 특히, 타워의 선택과 배치를 담당하는 책임을 `TowerPlacementController`와 `TowerWorldSelectionController`로 분리하여 **단일 책임 원칙(SRP)**을 준수하고자 노력했습니다.

---

## 📂 프로젝트 구조
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

## 📈 개발 과정 및 회고
  유니티 개발자로서 한 단계 성장하기 위해 시작한 개인 프로젝트입니다. 단순히 기능을 완성하는 것보다, 그동안 부족하다고 느꼈던 좋은 설계와 디자인 패턴, 클린 코드 원칙을 학습하고 직접 적용하는 과정에 가장 큰 의미를 두었습니다.
