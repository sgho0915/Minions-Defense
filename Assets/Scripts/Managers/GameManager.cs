using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Stage Settings")]
    public int stageIndex;

    [Header("Dependencies")]
    public WaveManager waveManager;
    public MainTowerController mainTower;
    public StageUIManager stageUI;

    // 화폐
    [Header("Currency")]
    public int stagePoints;      // 스테이지 내에서만 쓰는 포인트
    public int globalPoints;     // 계정 단위 영구 포인트

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        // 1) 초기화
        stagePoints = 100;       // 예시: 스테이지 시작할 때 100P 지급
        globalPoints = PlayerPrefs.GetInt("GlobalPoints", 0);

        // UI 초기화
        stageUI.Initialize(mainTower.MaxHp, stagePoints, globalPoints);

        // 2) 이벤트 구독
        mainTower.OnDied += OnStageFail;

        // 3) 게임 루프 시작
        StartCoroutine(RunStage());
    }

    private IEnumerator RunStage()
    {
        // 잠깐 대기
        yield return null;

        // 웨이브 진행
        yield return StartCoroutine(waveManager.RunWaves());

        // 웨이브가 모두 끝나도 살아있다면 클리어
        if (mainTower.CurrentHp > 0)
            OnStageClear();
    }

    private void OnStageFail()
    {
        StopAllCoroutines();
        stageUI.ShowGameOver(false);
    }

    private void OnStageClear()
    {
        StopAllCoroutines();

        // 성능에 따라 별 계산 (예시: 남은 HP 비율)
        float hpRatio = (float)mainTower.CurrentHp / mainTower.MaxHp;
        int stars = hpRatio > .75f ? 3 : hpRatio > .5f ? 2 : 1;

        // 스테이지 포인트를 글로벌 포인트로 전환 (예: stagePoints * stars)
        int reward = stagePoints * stars;
        globalPoints += reward;
        PlayerPrefs.SetInt("GlobalPoints", globalPoints);
        PlayerPrefs.SetInt($"Stage_{stageIndex}_Stars", stars);
        PlayerPrefs.Save();

        stageUI.ShowGameOver(true, stars, reward);
    }

    // 타워 건설 등에서 호출
    public bool TrySpendStagePoints(int cost)
    {
        if (stagePoints < cost) return false;
        stagePoints -= cost;
        stageUI.UpdateStagePoints(stagePoints);
        return true;
    }
}
