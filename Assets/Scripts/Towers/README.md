# Towers 폴더 구조
Towers/
├── Data/
│ ├ TowerDataSO.cs – ScriptableObject 정의
│ └ TowerLevelData.cs – 레벨별 데이터 모델
├── Interfaces/
│ └ ITower.cs – 타워 공통 동작 인터페이스
├── Factory/
│ └ TowerFactory.cs – 팩토리 패턴으로 타워 생성
├── Controllers/
│ └ TowerController.cs – 공격·업그레이드·판매 로직
├── Enums/
│ ├ TowerType.cs - 타워 타입
│ ├ TowerLevel4Type.cs - 레벨4 강화 A/B타입 분기
│ ├ TowerAreaShape.cs - AoE(광역 공격 효과) 형태
│ └ TowerTargetPriority.cs - 타겟 우선 순위
├── ScriptableObjects/ – 에디터에서 생성된 SO 에셋 모음
└── Prefabs/ – 타워 프리팹(모델+스크립트 컴포넌트)


## 간략 설명

- **Data/**  
  순수 데이터 클래스와 SO만 모아둡니다.  
- **Interfaces/**  
  ITower 인터페이스 등, 각 모듈 간 계약을 정의.  
- **Factory/**  
  TowerFactory를 통해 일관된 생성 로직 제공.  
- **Controllers/**  
  TowerController가 `ITower`를 구현하여 실제 동작을 수행.  
- **Enums/**  
  프로젝트 전역에서 쓰이는 열거형 정의.  
- **ScriptableObjects/**  
  에디터에서 생성된 `.asset` 파일들이 모이는 곳(실제 에셋 폴더).  
- **Prefabs/**  
  TowerDataSO와 TowerController가 붙어 있는 실제 프리팹 파일 모음.