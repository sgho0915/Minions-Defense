using UnityEngine;

/// <summary>
/// ScriptableObject로 에디터에서 관리되는 타워 정적 데이터 집합
/// </summary>

[CreateAssetMenu(fileName = "TowerData", menuName = "TowerDefenseAssets/Tower/TowerDataSO")]
public class TowerDataSO : ScriptableObject
{
    [Header("타워 식별 및 기본값")]
    public string towerName;        // 타워 이름
    public TowerType towerType;     // 타워 타입
    public GameObject towerPrefab;  // 타워 프리팹

    [Header("레벨별 데이터 (1~4, 4단계는 A/B 포함)")]
    public TowerLevelData[] levelData;
}

/// <summary>
/// 타워 구분용 타입 열거형
/// </summary>
public enum TowerType
{
    Mage,
    Cannon,
    DogBarrack,
    MachineGun,
    Main
}