using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// Addressable 기반으로 적 생성
public class EnemySpawner : MonoBehaviour
{
    [Header("스폰할 적 데이터")]
    public EnemyData[] enemyDatas;

    public void SpawnEnemy(int index)
    {
        if (index < 0 || index >= enemyDatas.Length) return;
        EnemyData enemyData = enemyDatas[index];

        // Addressable 사용시
        //StartCoroutine(SpawnRoutine(enemyData));

        // 프리팹 직접 참조 후 인스턴스화
        GameObject go = Instantiate(enemyData.prefab, transform.position, Quaternion.identity);
        Enemy enemy = go.GetComponent<Enemy>();
        enemy.Init(enemyData);
    }

    // Addressable 사용시
    //private IEnumerator SpawnRoutine(EnemyData enemyData)
    //{
    //    AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(enemyData.prefabAddress);
    //    yield return handle;

    //    if (handle.Status == AsyncOperationStatus.Succeeded)
    //    {
    //        GameObject go = Instantiate(handle.Result, transform.position, Quaternion.identity);
    //        Enemy enemy = go.GetComponent<Enemy>();
    //        enemy.Init(enemyData);
    //    }
    //    else
    //    {
    //        Debug.LogError("프리팹 로드 실패: " + enemyData.prefabAddress);
    //    }
    //}
}
