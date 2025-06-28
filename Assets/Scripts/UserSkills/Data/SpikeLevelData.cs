// SpikeLevelData.cs
using UnityEngine;

/// <summary>
/// “스파이크” 스킬의 레벨별 데이터
/// </summary>
[System.Serializable]
public class SpikeLevelData
{
    [Header("공통")]
    public CommonSkillLevelData commonSkillData;

    [Header("트랩 설정")]
    [Tooltip("유지 시간 (초)")]
    public float trapDuration;
    [Tooltip("트랩 반경")]
    public float trapRadius;
    [Tooltip("초당 데미지")]
    public int damagePerSecond;
    [Tooltip("감속 비율 (0~1)")]
    public float slowAmount;
    [Tooltip("감속 지속 시간 (초)")]
    public float slowDuration;

    [Header("연출")]
    [Tooltip("트랩 이펙트 Prefab")]
    public GameObject trapEffectPrefab;
    [Tooltip("트랩 사운드 클립")]
    public AudioClip trapSoundClip;
}
