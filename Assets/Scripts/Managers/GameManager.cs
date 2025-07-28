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
    private StageUIController stageUI;

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
        stageUI = FindObjectOfType<StageUIController>();

        // 초기값 세팅
        stagePoints = 100;
        globalPoints = PlayerPrefs.GetInt("GlobalPoints", 0);

        // UI 초기화
        stageUI.Initialize(mainTower, waveManager, stagePoints);

        // 이벤트 구독
        mainTower.OnDied += HandleStageFail;

        // 본격 게임 흐름 시작
        StartCoroutine(RunStage());
    }

    private IEnumerator RunStage()
    {
        // 초기화 보장용 1프레임 대기
        yield return null;

        // 웨이브 시작
        yield return StartCoroutine(waveManager.RunWaves());

        // 모든 웨이브 스폰 및 남은 몬스터 없는지 대기
        yield return new WaitUntil(() =>
        waveManager.CurrentWaveIndex >= waveManager.waveDataSO.waves.Length - 1  // 모든 웨이브 소진
        && waveManager.transform.childCount == 0                             // 스폰된 몬스터 제거 완료
        );

        // 모든 웨이브 몬스터가 처치될 때까지 타워가 살아있으면 클리어
        if (mainTower.CurrentHp > 0)
            HandleStageClear();
    }

    private void HandleStageFail()
    {
        StopAllCoroutines();

        bool[] failCriteria = new bool[3] { false, false, false };
        stageUI.ShowResult(false, failCriteria);
    }

    private void HandleStageClear()
    {
        StopAllCoroutines();

        // 성능에 따라 별 계산 (예시: 남은 HP 비율)
        float hpRatio = (float)mainTower.CurrentHp / mainTower.MaxHp;
        int stars = hpRatio > .75f ? 3 : hpRatio > .5f ? 2 : 1;

        // 각 평가 기준 충족 여부 예시 (실제 로직에 맞춰 수정)
        bool[] criteriaMet = new bool[3]
        {
            stars >= 1, // 기준1: 최소 1성 달성
            stars >= 2, // 기준2: 최소 2성 달성
            stars >= 3  // 기준3: 3성 달성
        };

        // 스테이지 포인트를 글로벌 포인트로 전환 (예: stagePoints * stars)
        int reward = stagePoints * stars;
        globalPoints += reward;
        PlayerPrefs.SetInt("GlobalPoints", globalPoints);
        PlayerPrefs.SetInt($"Stage_{stageIndex}_Stars", stars);
        PlayerPrefs.Save();

        
        stageUI.ShowResult(true, criteriaMet);
    }

    // 타워 건설 등에서 호출
    public bool TrySpendStagePoints(int cost)
    {
        if (stagePoints < cost) return false;
        stagePoints -= cost;
        //stageUI.hudView.UpdateStagePoints(stagePoints);
        return true;
    }
}
