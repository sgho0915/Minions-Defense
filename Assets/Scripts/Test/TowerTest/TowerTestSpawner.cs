using UnityEngine;

public class TowerTestSpawner : MonoBehaviour
{
    public TowerDataSO canonData;
    public MonsterDataSO monsterData;
    public Transform towerSpawnPoint;
    public Transform monsterSpawnPoint;
    private TowerController lastTower;
    private MonsterController monsterController;

    public void SpawnLevel1Tower()
    {
        // levelData 배열의 첫 요소로 소환
        var go = TowerFactory.Instance
                    .CreateTower(canonData.levelData[0], canonData.levelData, towerSpawnPoint.position, null);
        lastTower = go.GetComponent<TowerController>();
    }

    public void SpawnMonster()
    {
        // levelData 배열의 첫 요소로 소환
        var go = MonsterFactory.Instance.CreateMonster(monsterData.levelData, MonsterSize.Small, monsterSpawnPoint.position, null);
        monsterController = go.GetComponent<MonsterController>();
    }
}
