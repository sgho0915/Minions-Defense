using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Stage Settings")]
    public int stageIndex;

    // Dependencies, 각 Stage 씬 롣 후 Find로 할당
    private WaveManager waveManager;
    private MainTowerController mainTower;
    private StageUIManager stageUI;

    // 화폐
    [Header("Currency")]
    public int stagePoints;      // 스테이지 내에서만 쓰는 포인트
    public int globalPoints;     // 계정 단위 영구 포인트

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 씬 변경 시 마다 콜백 받기
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // "Stage_" 로 시작하는 씬이면만 초기화
        if (!scene.name.StartsWith("Stage_")) return;

        // 씬 안의 의존성 컴포넌트들 찾아오기
        waveManager = FindObjectOfType<WaveManager>();
        mainTower = FindObjectOfType<MainTowerController>();
        stageUI = FindObjectOfType<StageUIManager>();

        // 초기값 세팅
        stagePoints = 100;
        globalPoints = PlayerPrefs.GetInt("GlobalPoints", 0);

        // UI 초기화
        stageUI.Initialize(mainTower.MaxHp, stagePoints, globalPoints);

        // 이벤트 구독
        mainTower.OnDied += OnStageFail;

        // 본격 게임 흐름 시작
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
