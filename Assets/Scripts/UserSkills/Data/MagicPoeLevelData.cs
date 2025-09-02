// MagicPoeLevelData.cs
using UnityEngine;

/// <summary>
/// 공통 스킬 데이터 SkillLevelData를 상속받은 “마법의 포댕이” 스킬 레벨별 데이터
/// </summary>
[System.Serializable]
public class MagicPoeLevelData : SkillLevelData
{
    [Header("포댕이 유닛 설정")]
    [Tooltip("이동 속도 (유닛/초)")]
    public float moveSpeed;

    [Tooltip("최대 피해량 (이만큼 피해를 입히면 소멸)")]
    public int maxDamageOutput; // 포댕이가 사라지기 전까지 가할 수 있는 총 데미지량 (포댕이의 체력 개념)

    [Header("플라즈마 오라(Aura) 설정")]    
    [Tooltip("공격 오라 반경")]
    public float auraRadius;    // 주변 몬스터를 감지할 공격 범위

    [Tooltip("초당 공격 횟수 (Tick Rate)")]
    public float damageTickRate;    // 초당 공격 횟수 (예: 4.0이면 1초에 4번, 즉 0.25초마다 공격)

    [Tooltip("틱당 데미지")]
    public int damagePerTick;    // 1회 공격(Tick)당 데미지

    [Tooltip("스턴 지속 시간 (초)")]
    public float stunDuration;

    [Header("연출")]
    [Tooltip("공격 오라 이펙트 Prefab (자식으로 붙여 사용)")]
    public GameObject auraEffectPrefab;    // 지속적으로 표시될 오라 이펙트

    [Tooltip("공격 시 몬스터에게 표시될 이펙트 Prefab")]
    public GameObject attackEffectPrefab;

    [Tooltip("공격 사운드 클립")]
    public AudioClip attackSoundClip;
}