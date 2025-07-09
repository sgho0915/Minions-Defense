// MonsterLevelData.cs
using UnityEngine;

[System.Serializable]
public class MonsterLevelData
{
    [Header("기본 정보")]
    public GameObject monsterPrefab;      // 몬스터 프리팹
    public MonsterSize size;              // 몬스터 크기(레벨) 구분: Small/Medium/Large
    public int maxHp;                     // 몬스터 최대 체력
    public float moveSpeed;               // 이동 속도 (유닛 단위/초)
    public int attackPower;               // 공격력 (근접 또는 투사체 피해량)

    [Header("공격 설정")]
    public bool isRanged;                 // 원거리 공격 여부 (true면 투사체 사용, false면 근접 공격)
    public float attackRange;             // 공격 사정거리
    public GameObject projectilePrefab;   // 원거리 공격 시 발사할 투사체 프리팹
    public float projectileSpeed;         // 투사체 비행 속도 (유닛 단위/초)

    [Header("보상")]
    public int rewardPointsMin;           // 고정 보상 포인트(랜덤 미사용 시)
    public int rewardPointsMax;           // 랜덤 보상 최대값(랜덤 보상 시 사용)
}