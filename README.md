# 🎮 Minions-Defense

![Unity](https://img.shields.io/badge/Unity-6000.1.7f1-blue)

**Unity 기반의 타워디펜스 개인 포트폴리오 예제**입니다.  
적 웨이브, 타워 업그레이드, 이벤트 채널 기반 UI 관리 등 게임 개발의 주요 요소를 담았습니다.

---

## 📺 데모 영상

[![데모 영상](https://utfs.io/f/nGnSqDveMsqx869v6NfsgcqER67P9XGtSfe5dyInlUv8us1K)](https://www.youtube.com/watch?v=QpGhH3YrmQY)

---

## 📺 스크린샷

### 웨이브 시스템
<img width="194" height="188" alt="Image" src="https://github.com/user-attachments/assets/cdbe7d8f-edfb-4066-9139-efca8185ce0b" />


---

## ⚙️ 주요 기능

-   **웨이브 시스템**
    -   `WaveManager` 클래스에서 스폰 간격·난이도 조절
-   **타워 건설 & 업그레이드**
    -   골드 사용, 레벨별 스탯 상승
-   **ScriptableObject 이벤트 채널**
    -   UI와 게임 로직 간 의존성 최소화
-   **Object Pooling**
    -   메모리 할당·해제 최소화로 모바일 퍼포먼스 최적화
-   **Addressables**
    -   리소스 그룹화 및 원격 로드 지원

---

## 🤝 브랜치 전략 (Git Flow)

-   **`main`**
    -   배포 전용 브랜치
    -   태그된 릴리즈만 존재
-   **`develop`**
    -   통합 개발 브랜치
    -   모든 기능이 이곳으로 머지됨
-   **`feature/<기능명>`**
    -   개별 기능 개발용 브랜치
    -   `develop` → 분기 → 기능 구현 → `develop`에 머지 → 삭제
-   **`release/vX.Y.Z`**
    -   릴리즈 준비용 브랜치
    -   `develop` → 분기 → 버전·문서·QA → `main`·`develop`에 머지 → 삭제
-   **`hotfix/vX.Y.Z`**
    -   긴급 버그 수정용 브랜치
    -   `main` → 분기 → 수정 → `main`·`develop`에 머지 → 삭제

---

## 🤖 코드 스타일

-   **C# 네이밍**
    -   클래스·열거형: `PascalCase`
    -   메서드: `PascalCase`
    -   변수·필드: `camelCase`
    -   상수: `ALL_CAPS`
