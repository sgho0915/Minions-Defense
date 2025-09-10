// StageUIManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class StageUIController : MonoBehaviour
{
    [Header("View 참조 요소")]
    [SerializeField] private StageHUDView hudView;
    [SerializeField] private TowerSkillListView towerSkillListView;
    [SerializeField] private StagePauseView pauseView;
    
    private MainTowerController mainTower;
    private WaveManager waveManager;

    private Coroutine _nextWaveTimerRoutine;

    private void OnEnable()
    {
        towerSkillListView.OnForceStartWaveButtonClicked += HandleForceStartWave;
    }

    private void OnDisable()
    {
        towerSkillListView.OnForceStartWaveButtonClicked -= HandleForceStartWave; 
        waveManager.OnWaveIdxChanged -= HandleWaveChanged;
        waveManager.OnWaveSpawnCompleted -= HandleWaveSpawnCompleted;
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
        hudView.InitStagePause();

        mainTower.OnHpChanged += hudView.UpdateHp;
        waveManager.OnWaveIdxChanged += hudView.UpdateWave;
        waveManager.OnWaveIdxChanged += HandleWaveChanged;
        waveManager.OnWaveSpawnCompleted += HandleWaveSpawnCompleted;
        GameManager.Instance.OnStagePointsChanged += hudView.UpdateStagePoints;
    }

    private void HandleForceStartWave()
    {
        waveManager.ForceStartNextWave();

        // 카운트다운 코루틴 중단 & 0 표시
        if (_nextWaveTimerRoutine != null)
            StopCoroutine(_nextWaveTimerRoutine);
        towerSkillListView.UpdateNextWaveTimer(0f);

        // 버튼 비활성화
        towerSkillListView.SetNextWaveTimeUIInteractable(false, false);
    }

    private void HandleWaveChanged(int curWave, int maxWave)
    {
        // 웨이브 시작할 때마다 버튼 재활성화
        if(curWave == maxWave)
            towerSkillListView.SetNextWaveTimeUIInteractable(false, true);
        else
            towerSkillListView.SetNextWaveTimeUIInteractable(false, false);

        // 이전 웨이브에 대한 남은 시간 코루틴 중단
        if (_nextWaveTimerRoutine != null)
            StopCoroutine(_nextWaveTimerRoutine);
    }

    private void HandleWaveSpawnCompleted(int curWave)
    {
        if (waveManager.waveDataSO.waves.Length <= curWave) return; // 마지막 웨이브면 수행 X

        towerSkillListView.SetNextWaveTimeUIInteractable(true, false);

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
            towerSkillListView.UpdateNextWaveTimer(remain);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 0초가 되면 뷰에도 0 표시
        towerSkillListView.UpdateNextWaveTimer(0f);
    }
}
