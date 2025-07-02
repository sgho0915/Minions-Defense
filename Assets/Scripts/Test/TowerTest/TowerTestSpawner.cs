using UnityEngine;

public class TowerTestSpawner : MonoBehaviour
{
    public TowerDataSO canonData;
    public Transform spawnPoint;
    private TowerController lastTower;

    public void SpawnLevel1Tower()
    {
        // levelData 배열의 첫 요소로 소환
        var go = TowerFactory.Instance
                    .CreateTower(canonData.levelData[0], canonData.levelData, spawnPoint.position, null);
        lastTower = go.GetComponent<TowerController>();
    }
}
