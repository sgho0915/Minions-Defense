using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyTestManager : MonoBehaviour
{
    [SerializeField] private EnemySpawner spawner;

    private void Update()
    {
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            spawner.SpawnEnemy(0);  // enemyDatas[0] 소환

        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            spawner.SpawnEnemy(1);  // enemyDatas[1] 소환

        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            spawner.SpawnEnemy(2);  // enemyDatas[2] 소환
    }
}
