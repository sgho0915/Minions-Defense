// MagicPoeLevelData.cs
using UnityEngine;

/// <summary>
/// “마법의 포댕이” 스킬의 레벨별 데이터
/// </summary>
[System.Serializable]
public class MagicPoeLevelData
{
    [Header("공통")]
    public int level;                   // 스킬 레벨 (1~3)    
    public int upgradeCost;             // 강화 비용    
    public int executeCost;             // 스킬 시전 비용    
    public GameObject modelPrefab;      // 모델 Prefab (투사체 또는 트랩)    
    public float cooldown;              // 쿨다운(초)
    public Sprite skillIcon;            // 스킬 아이콘
    public string skillDesc;            // 스킬 설명

    [Header("포댕이 유닛 설정")]
    [Tooltip("이동 속도 (유닛/초)")]
    public float moveSpeed;
    [Tooltip("충돌 판정 반경")]
    public float collideRadius;
    [Tooltip("기본 데미지")]
    public int baseDamage;
    [Tooltip("스턴 지속 시간 (초)")]
    public float stunDuration;

    [Header("연출")]
    [Tooltip("이동 애니메이션 클립")]
    public AnimationClip moveAnim;
    [Tooltip("공격 이펙트 Prefab")]
    public GameObject attackEffectPrefab;
    [Tooltip("공격 사운드 클립")]
    public AudioClip attackSoundClip;
}