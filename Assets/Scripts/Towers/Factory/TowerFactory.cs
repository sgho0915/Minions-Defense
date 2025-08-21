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
    public TowerController CreateTower(TowerDataSO so, TowerLevelData initial, Vector3 pos, Transform parent)
    {
        var go = Object.Instantiate(initial.towerPrefab, pos, Quaternion.identity, parent);
        var ctrl = go.GetComponent<TowerController>() ?? go.AddComponent<TowerController>();
        ctrl.Initialize(so, initial, so.levelData);   // DataSO 포함 초기화

        // 배치된 타워의 콜라이더 레이어를 Tower로 지정
        int towerLayer = LayerMask.NameToLayer("Tower");
        if (towerLayer != -1)
        {
            go.layer = towerLayer;
            foreach (var col in go.GetComponentsInChildren<Collider>(true))
                col.gameObject.layer = towerLayer;
        }

        return ctrl;
    }
}
