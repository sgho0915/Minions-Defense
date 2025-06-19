using UnityEngine;

/// <summary>
/// 몬스터 스탯, 스킬, 애니메이션 클립을 담는 SO
/// </summary>

public enum MonsterSize { Small, Meduim, Large }    // 몬스터 사이즈(소, 중, 대)
public enum AttackType { Melee, Ranged, Boss, Tank }    // 몬스터 공격 타입(단거리, 원거리, 보스몹, 탱커)

[CreateAssetMenu(fileName = "MonsterData", menuName = "TowerDefense/MonsterData")]
public class MonsterData : ScriptableObject
{
    [Header("식별 정보")]
    public string monsterKey;           // 몬스터 식별 키(Skeleton, Mole, Cactus, Spirit, Capitalist)
    public string monsterDesc;          // 몬스터 설명 정보

    [Header("능력치")]
    public MonsterSize size;            // 몬스터 사이즈
    public int maxHealth;               // HP
    public float moveSpeed;             // 이동속도
    public int attackPower;             // 공격력

    [Header("보상")]
    // 랜덤 보상 아닐 경우 min == max
    public int minRewardPoints;         // 최소 보상
    public int maxRewardPoints;         // 최대 보상

    [Header("공격 정보")]
    public AttackType attackType;       // 몬스터 공격 타입(단거리, 원거리, 보스몹, 탱커)
    [TextArea] public string note;      // 비고(단거리, 원거리, 보스몹, 탱커)
    public string skillName;            // 스킬명
    public float skillCoolTime;         // 공격 쿨타임

    [Header("애니메이션")]
    public AnimationClip walkClip;      // 이동 모션
    public AnimationClip attackClip;    // 공격 모션
    public AnimationClip deathClip;     // 사망 모션
}
