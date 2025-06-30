// SpawnEntry.cs
using UnityEngine;

[System.Serializable]
/// <summary>
/// 한 몬스터 종류 스폰 설정
/// </summary>
public class SpawnEntry
{
    [Tooltip("스폰할 몬스터 데이터")]
    public MonsterDataSO monsterData;
    [Tooltip("크기(레벨)")]
    public MonsterSize size;
    [Tooltip("스폰 개수")]
    public int count;
    [Tooltip("스폰 간격(초)")]
    public float spawnInterval;
}