// MonsterDataSO.cs
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "TowerDefenseAssets/Monster/MonsterDataSO")]
public class MonsterDataSO : ScriptableObject
{
    [Header("몬스터 식별 및 기본값")]
    public string monsterName;          // 몬스터 이름
    public MonsterType monsterType;     // 몬스터 타입
    public GameObject monsterPrefab;    // 몬스터 프리팹

    [Header("몬스터 스탯, 에셋")]
    public MonsterLevelData[] levelData;
}
