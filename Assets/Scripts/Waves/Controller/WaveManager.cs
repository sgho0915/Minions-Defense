using System.Collections;
using UnityEngine;

// WavaManager.cs
/// <summary>
/// WaveDataSO를 읽어 몬스터를 순차적으로 스폰하고 다음 웨이브로 넘기는 매니저
/// </summary>
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("웨이브 데이터")]
    public WaveDataSO waveDataSO;

    [Header("스폰 위치")]
    public Transform spawnPoint;

    [Header("경로(Path)")]
    public WayPath path;

    private int _currentWaveIndex = 0;

    private void Awake()
    {
        // 만약 씬에 중복으로 붙어 있으면 하나만 살아남도록
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // (원한다면) DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartCoroutine(RunWaves());
    }

    // 모든 웨이브를 순서대로 실행하고, 각 웨이브 사이에 지연을 둠
    private IEnumerator RunWaves()
    {
        var waves = waveDataSO.waves;
        while (_currentWaveIndex < waves.Length)
        {
            var wave = waves[_currentWaveIndex];
            // 현재 웨이브 스폰 실행
            yield return StartCoroutine(SpawnWave(wave));
            // 웨이브 완료 후 대기
            yield return new WaitForSeconds(wave.delayAfterWave);
            _currentWaveIndex++;
        }
        Debug.Log("모든 웨이브 완료!");
    }

    // 단일 웨이브 내의 모든 스폰 엔트리를 실행
    private IEnumerator SpawnWave(WaveInfo wave)
    {
        Debug.Log($"<Wave {wave.waveNumber}> 시작");
        foreach (var entry in wave.spawns)
        {
            for (int i = 0; i < entry.count; i++)
            {
                SpawnMonster(entry);
                yield return new WaitForSeconds(entry.spawnInterval);
            }
        }
    }

    // MonsterFactory를 통해 몬스터를 생성하고 이동 경로를 할당
    private void SpawnMonster(SpawnEntry entry)
    {
        // 1) 팩토리로 몬스터만 뽑아냄
        var go = MonsterFactory.Instance
            .CreateMonster(
                entry.monsterData.levelData[0],
                entry.monsterData.levelData,
                entry.size,
                spawnPoint.position,
                transform);
        Debug.Log($"Spawned: {entry.monsterData.monsterName} Size:{entry.size}");

        // 2) 사이즈(레벨) 적용부터 몬스터에게 맡김
        go.GetComponent<MonsterController>().SetSize(entry.size);
    }
}
