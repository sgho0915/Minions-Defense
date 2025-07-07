// MonsterFactory.cs
using UnityEngine;

/// <summary>
/// 몬스터 인스턴스 생성 전담 싱글톤
/// </summary>
public class MonsterFactory
{
    private static MonsterFactory instance;
    public static MonsterFactory Instance => instance ??= new MonsterFactory();
    private MonsterFactory() { }

    /// <summary>
    /// MonsterDataSO.mosterPrefab을 인스턴스화 하고 초기화
    /// </summary>
    public GameObject CreateMonster(MonsterLevelData initialLevel, MonsterLevelData[] allLevels, MonsterSize size, Vector3 pos, Transform parent = null)
    {
        var go = Object.Instantiate(initialLevel.monsterPrefab, pos, Quaternion.identity, parent);
        var ctrl = go.GetComponent<MonsterController>() ?? go.AddComponent<MonsterController>();
        ctrl.Initialize(initialLevel, allLevels);
        ctrl.SetSize(size);
        return go;
    }
}
