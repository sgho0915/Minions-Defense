// StageUIManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Collections;

public class StageUIController : MonoBehaviour
{
    [Header("View 참조 요소")]
    [SerializeField] private StageHUDView hudView;
    [SerializeField] private StageResultView resultView;
    [SerializeField] private TowerSkillListView towerListView;
    
    private MainTowerController mainTower;
    private WaveManager waveManager;

    private Coroutine _nextWaveTimerRoutine;

    private void OnEnable()
    {
        resultView.Hide();
        towerListView.OnForceStartWaveButtonClicked += HandleForceStartWave;
    }

    private void OnDisable()
    {
        towerListView.OnForceStartWaveButtonClicked -= HandleForceStartWave; 
        waveManager.OnWaveIdxChanged -= HandleWaveChanged;
        waveManager.OnWaveSpawnCompleted -= HandleWaveSpawncompleted;
        GameManager.Instance.OnStagePointsChanged -= hudView.UpdateStagePoints;
    }

    /// <summary>
    /// GameManager에서 호출 -> HUD 초기화 및 이벤트 연결
    /// </summary>
    public void Initialize(MainTowerController _mainTower, WaveManager _waveManager, int startStagePoints)
    {
        this.mainTower = _mainTower;
        this.waveManager = _waveManager;

        // 초기 HUD UI 세팅
        hudView.UpdateHp(mainTower.CurrentHp, mainTower.MaxHp);
        hudView.UpdateWave(waveManager.CurrentWaveIndex, waveManager.waveDataSO.waves.Length);
        hudView.UpdateStagePoints(startStagePoints);

        mainTower.OnHpChanged += hudView.UpdateHp;
        waveManager.OnWaveIdxChanged += hudView.UpdateWave;
        waveManager.OnWaveIdxChanged += HandleWaveChanged;
        waveManager.OnWaveSpawnCompleted += HandleWaveSpawncompleted;
        GameManager.Instance.OnStagePointsChanged += hudView.UpdateStagePoints;
    }

    /// <summary>
    /// 게임 종료 시 결과 화면 표시를 결과 뷰에 위임
    /// </summary>
    /// <param name="clear">클리어 여부</param>
    /// <param name="criteriaMet">평가기준 충족 여부 배열</param>
    public void ShowResult(bool clear, bool[] criteriaMet)
    {        
        resultView.ShowResult(clear, criteriaMet);
    }

    private void HandleForceStartWave()
    {
        waveManager.ForceStartNextWave();

        // 카운트다운 코루틴 중단 & 0 표시
        if (_nextWaveTimerRoutine != null)
            StopCoroutine(_nextWaveTimerRoutine);
        towerListView.UpdateNextWaveTimer(0f);

        // 버튼 비활성화
        towerListView.SetNextWaveTimeUIInteractable(false, false);
    }

    private void HandleWaveChanged(int curWave, int maxWave)
    {
        // 웨이브 시작할 때마다 버튼 재활성화
        if(curWave == maxWave)
            towerListView.SetNextWaveTimeUIInteractable(false, true);
        else
            towerListView.SetNextWaveTimeUIInteractable(false, false);

        // 이전 웨이브에 대한 남은 시간 코루틴 중단
        if (_nextWaveTimerRoutine != null)
            StopCoroutine(_nextWaveTimerRoutine);
    }

    private void HandleWaveSpawncompleted(int curWave)
    {
        if (waveManager.waveDataSO.waves.Length <= curWave) return; // 마지막 웨이브면 수행 X

        towerListView.SetNextWaveTimeUIInteractable(true, false);

        float delay = waveManager.waveDataSO.waves[curWave - 1].delayAfterWave;
        if (_nextWaveTimerRoutine != null)
            StopCoroutine(_nextWaveTimerRoutine);
        _nextWaveTimerRoutine = StartCoroutine(NextWaveCountdown(delay));
    }

    private IEnumerator NextWaveCountdown(float delay)
    {
        float elapsed = 0f;

        // delay만큼 또는 버튼 클릭(강제 시작)까지
        while (elapsed < delay)
        {
            float remain = delay - elapsed;
            towerListView.UpdateNextWaveTimer(remain);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 0초가 되면 뷰에도 0 표시
        towerListView.UpdateNextWaveTimer(0f);
    }
}
