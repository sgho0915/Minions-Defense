// MagicPoeLevelData.cs
using UnityEngine;

/// <summary>
/// “마법의 포댕이” 스킬의 레벨별 데이터
/// </summary>
[System.Serializable]
public class MagicPoeLevelData
{
    [Header("공통")]
    public CommonSkillLevelData commonSkillData;

    [Header("포댕이 유닛 설정")]
    [Tooltip("이동 속도 (유닛/초)")]
    public float moveSpeed;
    [Tooltip("충돌 판정 반경")]
    public float collideRadius;
    [Tooltip("기본 데미지")]
    public int baseDamage;
    [Tooltip("넉백 힘 (Impulse)")]
    public float knockbackForce;
    [Tooltip("스턴 지속 시간 (초)")]
    public float stunDuration;
    [Tooltip("연쇄 범위 (3단계 전용)")]
    public float chainRadius;

    [Header("연출")]
    [Tooltip("이동 애니메이션 클립")]
    public AnimationClip moveAnim;
    [Tooltip("피격 애니메이션 클립")]
    public AnimationClip attackAnim;
    [Tooltip("피격 이펙트 Prefab")]
    public GameObject hitEffectPrefab;
    [Tooltip("피격 사운드 클립")]
    public AudioClip hitSoundClip;
}