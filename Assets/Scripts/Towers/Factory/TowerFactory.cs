// TowerFactory.cs
using UnityEngine;

/// <summary>
/// TowerDataSO 기반으로 타워 프리팹 인스턴스 생성 (Singleton)
/// </summary>
public class TowerFactory
{
    private static TowerFactory instance;
    public static TowerFactory Instance => instance ??= new TowerFactory();
    private TowerFactory() { }

    /// <summary>
    /// TowerDataSO.towerPrefab을 인스턴스화 하고 초기화
    /// </summary>
    public GameObject CreateTower(TowerDataSO towerDataSO, Vector3 createPosition, Transform parent)
    {
        // Factory에서 기본 화전값으로 반환받은 인스턴스화된 타워 게임 오브젝트
        var towerRoot = Object.Instantiate(towerDataSO.towerPrefab, createPosition, Quaternion.identity, parent);
        var controller = towerRoot.GetComponent<TowerController>() ?? towerRoot.AddComponent<TowerController>();
        controller.Initialize(towerDataSO);
        return towerRoot;
    }
}
