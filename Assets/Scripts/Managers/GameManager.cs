using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action<int> OnStagePointsChanged;

    [Header("Stage Settings")]
    public int stageIndex;
    public bool isWaveStarted = false;

    private WaveManager waveManager;
    private MainTowerController mainTower;
    private StageUIController stageUIController;
    private StageUIManager stageUIManager;

    [Header("Currency")]
    public int stagePoints;      // 스테이지 내에서만 쓰는 포인트
    public int globalPoints;     // 계정 단위 영구 포인트

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        //  프레임 제한 해제
        Application.targetFrameRate = 120;

        //  절전 모드에서 최대 성능 유지
        QualitySettings.vSyncCount = 0; // VSync 비활성화

        globalPoints = PlayerPrefs.GetInt("GlobalPoints", 0);

        // 씬 변경 시 마다 콜백 받기
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.StartsWith("Stage_"))
            InitializeStageScene(scene);
    }

    private void InitializeStageScene(Scene scene)
    {
        // 씬 안의 의존성 컴포넌트들 찾아오기
        waveManager = FindObjectOfType<WaveManager>();
        mainTower = FindObjectOfType<MainTowerController>();
        stageUIController = FindObjectOfType<StageUIController>();
        stageUIManager = FindObjectOfType<StageUIManager>();

        // 초기값 세팅
        stagePoints = 300;
        isWaveStarted = false;

        // UI 초기화
        stageUIController?.Initialize(mainTower, waveManager, stagePoints);

        // 이벤트 구독
        mainTower.OnDied += HandleStageFail;
        waveManager.OnMonsterSpawned += HandleMonsterSpawned;
    }

    public void CleanupStageScene()
    {
        // 스테이지를 떠날 때 GameManager에서 실행되는 모든 코루틴 중지
        StopAllCoroutines();

        // 모든 이벤트 구독 해제
        mainTower.OnDied -= HandleStageFail;
        waveManager.OnMonsterSpawned -= HandleMonsterSpawned;

        // 모든 컴포넌트 참조 해제
        waveManager = null;
        mainTower = null;
        stageUIController = null;
        stageUIManager = null;

        // 스테이지 관련 변수 리셋
        isWaveStarted = false;
    }

    public IEnumerator RunStage()
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
        int reward = Mathf.RoundToInt(stagePoints * 0.3f); // 게임오버시에는 몬스터 처치 보상의 30%만 반올림 적용해 글로벌 포인트 보상
        globalPoints += reward;
        PlayerPrefs.SetInt("GlobalPoints", globalPoints);
        PlayerPrefs.SetInt($"Stage_{stageIndex}_Stars", 0);
        PlayerPrefs.Save();

        stageUIManager.ShowResultView(false, failCriteria, reward);
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
        int reward = Mathf.RoundToInt(stagePoints * stars * 0.5f); // 클리어 시 몬스터 처치 보상 * 별 등급의 50% 만큼 보상
        globalPoints += reward;
        PlayerPrefs.SetInt("GlobalPoints", globalPoints);
        PlayerPrefs.SetInt($"Stage_{stageIndex}_Stars", stars);
        PlayerPrefs.Save();

        stageUIManager.ShowResultView(true, criteriaMet, reward);
    }

    public void HandleMonsterSpawned(MonsterController mc)
    {
        mc.OnGiveReward += TryGiveStagePoints;
        mc.OnGiveReward += OnUnsubscribed;

        // 스스로 구독해제
        void OnUnsubscribed(int amount)
        {
            mc.OnGiveReward -= TryGiveStagePoints;
            mc.OnGiveReward -= OnUnsubscribed;  // 최후에는 구독해제 이벤트도 구독해제
        }
    }

    // 몬스터 처치, 타워 재판매로 인한 스테이지 포인트 보상
    public void TryGiveStagePoints(int amount)
    {
        stagePoints += amount;
        OnStagePointsChanged?.Invoke(stagePoints);
    }

    // 타워 건설, 강화 등에서 호출
    public bool TrySpendStagePoints(int cost)
    {
        if (stagePoints < cost) return false;
        stagePoints -= cost;
        OnStagePointsChanged?.Invoke(stagePoints);
        return true;
    }


}
