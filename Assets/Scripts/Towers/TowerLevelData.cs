using UnityEngine;
using static UnityEngine.GUILayout;

/// <summary>
/// 한 레벨(Lv1~4A/4B)에 해당하는 수치·설정 정보
/// </summary>

[System.Serializable]
public class TowerLevelData
{
    [Header("기본 정보")]
    [Tooltip("레벨 번호 (1~4)")]
    public int level;
    [Tooltip("4단계 분기 타입 (A, B)")]
    public TowerLevel4Type level4Type;


    [Header("UI 정보")]
    [Tooltip("레벨별 아이콘 (선택 UI에 레벨별 표시가 필요할 때)")]
    public Sprite levelIcon;
    [TextArea]
    [Tooltip("레벨별 설명 (툴팁 등)")]
    public string description;
    [Tooltip("레벨별 한 줄 소개 텍스트")]
    public string flavorText;


    [Header("전투 기본 정보")]
    [Tooltip("데미지")]
    public float damage;
    [Tooltip("공격 속도 (초당)")]
    public float attackSpeed;
    [Tooltip("사정 거리 (단일 대상 인식")]
    public float range;


    [Header("투사체 설정")]
    [Tooltip("발사체(Projectile) 프리팹")]
    public GameObject projectilePrefab;
    [Tooltip("투사체 속도")]
    public float projectileSpeed;


    [Header("광역 효과 (AoE)")]
    [Tooltip("광역 반경 피해 (0 = 단일 대상)")]
    public float splashRadius;
    [Tooltip("광역 효과 모양 (Circle, Cone, Line)")]
    public AreaShape areaShape;
    [Tooltip("원뿔형일 때 각도 (Degree)")]
    public float coneAngle;
    [Tooltip("직선형일 때 길이")]
    public float lineLength;


    [Header("상태 이상 & 크리티컬")]
    [Range(0, 1)]
    [Tooltip("크리티컬 확률")]
    public float critChance;
    [Tooltip("크리티컬 데미지 배수")]
    public float critMultiplier;
    [Range(0, 1)]
    [Tooltip("슬로우 확률")]
    public float slowChance;
    [Tooltip("슬로우 지속 시간 (초)")]
    public float slowDuration;
    [Range(0, 1)]
    [Tooltip("스턴 확률")]
    public float stunChance;
    [Tooltip("스턴 지속 시간 (초)")]
    public float stunDuration;


    [Header("타겟팅 설정")]
    [Tooltip("타겟 우선순위 (First, Last, Strongest, Weakest, Closest)")]
    public TargetPriority targetPriority;
    [Tooltip("타겟 레이어 마스크")]
    public LayerMask targetLayerMask;
    [Tooltip("타겟 태그 필터")]
    public string[] targetTags;


    [Header("비용 & 시간")]
    [Tooltip("업그레이드 비용 (포인트)")]
    public int upgradeCost;
    [Tooltip("되팔기 가격")]
    public int sellPrice;


    [Header("시각 & 음향 연출")]
    [Tooltip("공격 이펙트 Prefab (없으면 None)")]
    public GameObject attackEffectPrefab;
    [Tooltip("공격 사운드 클립")]
    public AudioClip attackSoundClip;
    [Tooltip("업그레이드 사운드 클립")]
    public AudioClip upgradeSoundClip;
    [Tooltip("설치 이펙트 Prefab")]
    public ParticleSystem buildEffect;
    [Tooltip("설치 사운드 클립")]
    public AudioClip buildSoundClip;
    [Tooltip("파괴 이펙트 Prefab")]
    public ParticleSystem destroyEffect;
    [Tooltip("파괴 사운드 클립")]
    public AudioClip destroySoundClip;
}

/// <summary>
/// 4단계에서 A/B 타입 구분을 위한 열거형
/// </summary>
public enum TowerLevel4Type
{
    None, A, B
}

/// <summary>
/// 광역 효과 형태
/// </summary>
public enum AreaShape
{
    None, Circle, Cone, Line
}

/// <summary>
/// 타겟 우선순위 옵션
/// </summary>
public enum TargetPriority
{
    First, Last, Strongest, Weakest, Closest
}