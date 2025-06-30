// TowerFactory.cs
using UnityEngine;

/// <summary>
/// TowerLevelData와 전체 레벨 배열을 받아 타워 프리팹을 생성하고 초기화하는 싱글톤 팩토리
/// </summary>
public class TowerFactory
{
    private static TowerFactory instance;
    public static TowerFactory Instance => instance ??= new TowerFactory();
    private TowerFactory() { }

    /// <summary>
    /// 지정된 레벨 데이터(TowerLevelData)로부터 타워를 인스턴스화하고 초기화합니다.
    /// </summary>
    /// <param name="levelData">타워 레벨별 데이터</param>
    /// <param name="createPosition">생성 위치</param>
    /// <param name="parent">부모 Transform</param>
    public GameObject CreateTower(TowerLevelData initialLevel, TowerLevelData[] allLevels, Vector3 createPosition, Transform parent)
    {
        // Factory에서 기본 화전값으로 반환받은 인스턴스화된 타워 게임 오브젝트
        var towerRoot = Object.Instantiate(initialLevel.towerPrefab, createPosition, Quaternion.identity, parent);

        // 컨트롤러가 없으면 붙여주고, 초기화는 컨트롤러 책임으로 위임
        var controller = towerRoot.GetComponent<TowerController>() ?? towerRoot.AddComponent<TowerController>();

        // 레벨별 데이터(TowerLevelData)를 직접 넘겨 초기화
        controller.Initialize(initialLevel, allLevels);
        return towerRoot;
    }
}
