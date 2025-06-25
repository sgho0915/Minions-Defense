// TowerLevelData.cs
using UnityEngine;

/// <summary>
/// 한 레벨(Lv1~4A/4B)에 해당하는 수치·설정 정보
/// TowerDataSO에 직렬화되는 데이터이므로 TowerLevelData도 같이 공유돼 메모리 중복 X
/// </summary>

[System.Serializable]
public class TowerLevelData
{
    [Header("기본 정보")]
    public int level;                   // 레벨 번호 (1~4)
    public TowerLevel4Type level4Type;  // 4단계 분기 타입 (A, B)


    [Header("UI 정보")]
    public Sprite levelIcon;            // 레벨별 아이콘 (선택 UI에 레벨별 표시가 필요할 때)
    [TextArea]
    public string description;          // 레벨별 설명 (툴팁 등)
    public string flavorText;           // 레벨별 한 줄 소개 텍스트


    [Header("전투 기본 정보")]
    public float damage;                // 데미지
    public float attackSpeed;           // 공격 속도 (초당)
    public float range;                 // 사정 거리 (단일 대상 인식)


    [Header("투사체 설정")]
    public GameObject projectilePrefab; // 발사체(Projectile) 프리팹
    public float projectileSpeed;       // 투사체 속도


    [Header("광역 효과 (AoE)")]
    public float splashRadius;          // 광역 반경 피해 (0 = 단일 대상)
    public TowerAreaShape areaShape;    // 광역 효과 모양 (Circle, Cone, Line)
    public float coneAngle;             // 원뿔형일 때 각도 (Degree)
    public float lineLength;            // 직선형일 때 길이


    [Header("상태 이상 & 크리티컬")]
    [Range(0, 1)]
    public float critChance;            // 크리티컬 확률
    public float critMultiplier;        // 크리티컬 데미지 배수
    [Range(0, 1)]
    public float slowChance;            // 슬로우 확률
    public float slowDuration;          // 슬로우 지속 시간 (초)
    [Range(0, 1)]
    public float stunChance;            // 스턴 확률
    public float stunDuration;          // 스턴 지속 시간 (초)


    [Header("타겟팅 설정")]
    public TowerTargetPriority targetPriority;  // 타겟 우선순위 (First, Last, Strongest, Weakest, Closest)
    public LayerMask targetLayerMask;           // 타겟 레이어 마스크
    public string[] targetTags;                 // 타겟 태그 필터


    [Header("비용 & 시간")]
    public int upgradeCost;             // 업그레이드 비용 (포인트)
    public int sellPrice;               // 되팔기 가격


    [Header("시각 & 음향 연출")]
    public GameObject attackEffectPrefab;   // 공격 이펙트 Prefab (없으면 None)
    public AudioClip attackSoundClip;       // 공격 사운드 클립
    public AudioClip upgradeSoundClip;      // 업그레이드 사운드 클립
    public ParticleSystem buildEffect;      // 설치 이펙트 Prefab
    public AudioClip buildSoundClip;        // 설치 사운드 클립
    public ParticleSystem destroyEffect;    // 파괴 이펙트 Prefab
    public AudioClip destroySoundClip;      // 파괴 사운드 클립
}