using System;
using System.Collections;
using UnityEngine;

// WavaManager.cs
/// <summary>
/// WaveDataSO를 읽어 몬스터를 순차적으로 스폰하고 다음 웨이브로 넘기는 매니저
/// </summary>
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    public Action<int, int> OnWaveIdxChanged;

    [Header("웨이브 데이터")]
    public WaveDataSO waveDataSO;

    [Header("스폰 위치")]
    public Transform spawnPoint;

    [Header("경로(Path)")]
    public WayPath path;

    private int _currentWaveIndex = -1;
    public int CurrentWaveIndex => _currentWaveIndex; // StageUIManager에서 읽기전용으로 참조하기 위한 캡슐화

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

    // 모든 웨이브를 순서대로 실행하고, 각 웨이브 사이에 지연을 둠
    public IEnumerator RunWaves()
    {
        var waves = waveDataSO.waves;
        int total = waves.Length;
        for (int i = 0; i < total; i++)
        {
            // 1) 내부 인덱스 갱신 ★★★
            _currentWaveIndex = i;

            // 2) 이벤트 호출 (1-based) ★★★
            OnWaveIdxChanged?.Invoke(i + 1, total);

            Debug.Log($"<Wave {waves[i].waveNumber}> 시작");

            // 3) 해당 웨이브 스폰 실행
            yield return StartCoroutine(SpawnWave(waves[i]));

            // 4) 웨이브 딜레이 적용 ★★★
            yield return new WaitForSeconds(waves[i].delayAfterWave);
        }
        Debug.Log("모든 웨이브 완료!");
    }

    // 단일 웨이브 내의 모든 스폰 엔트리를 실행
    private IEnumerator SpawnWave(WaveInfo wave)
    {
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
                entry.monsterData.levelData,
                entry.size,
                spawnPoint.position,
                transform);

        // 2) 사이즈(레벨) 적용부터 몬스터에게 맡김
        go.GetComponent<MonsterController>().SetSize(entry.size);
    }
}
