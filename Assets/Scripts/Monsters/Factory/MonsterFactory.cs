// MonsterFactory.cs
using System.Linq;
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
    public GameObject CreateMonster(    
    MonsterLevelData[] allLevels,
    MonsterSize size,
    Vector3 pos,
    Transform parent = null)
    {
        // 요청 받은 size 데이터에 맞는 레벨 데이터를 initialLevel로 사용
        var initialLevel = allLevels.First(l => l.size == size);

        var go = Object.Instantiate(initialLevel.monsterPrefab, pos, Quaternion.identity, parent);
        var ctrl = go.GetComponent<MonsterController>() ?? go.AddComponent<MonsterController>();
        ctrl.Initialize(initialLevel, allLevels);
        return go;
    }
}
