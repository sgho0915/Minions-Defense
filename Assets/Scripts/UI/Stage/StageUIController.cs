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
    
    private MainTowerController mainTower;
    private WaveManager waveManager;

    private Coroutine _nextWaveTimerRoutine;

    private void OnEnable()
    {
        resultView.Hide();
        hudView.OnForceStartWaveButtonClicked += HandleForceStartWave;
    }

    private void OnDisable()
    {
        hudView.OnForceStartWaveButtonClicked -= HandleForceStartWave; 
        waveManager.OnWaveIdxChanged -= HandleWaveChanged;        
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
    }

    /// <summary>
    /// 게임 종료 시 결과 화면 표시를 결과 뷰에 위임
    /// </summary>
    /// <param name="clear">클리어 여부</param>
    /// <param name="criteriaMet">평가기준 충족 여부 배열</param>
    public void ShowResult(bool clear, bool[] criteriaMet)
    {
        Debug.Log($"[UIController] ShowResult called: clear={clear}"); 
        resultView.ShowResult(clear, criteriaMet);
    }

    private void HandleForceStartWave()
    {
        waveManager.ForceStartNextWave();

        // 카운트다운 코루틴 중단 & 0 표시
        if (_nextWaveTimerRoutine != null)
            StopCoroutine(_nextWaveTimerRoutine);
        hudView.UpdateNextWaveTimer(0f);

        // 버튼 비활성화
        hudView.SetNextWaveTimeUIInteractable(false, false);
    }

    private void HandleWaveChanged(int curWave, int maxWave)
    {
        // 웨이브 시작할 때마다 버튼 재활성화
        if(curWave == maxWave) 
            hudView.SetNextWaveTimeUIInteractable(false, true);
        else
            hudView.SetNextWaveTimeUIInteractable(true, false);

        // 이전 웨이브에 대한 남은 시간 코루틴 중단
        if (_nextWaveTimerRoutine != null)
            StopCoroutine(_nextWaveTimerRoutine);

        // 새 웨이브에 대한 delayAfterWave 값으로 코루틴 새로 시작
        float delay = waveManager.waveDataSO.waves[curWave - 1].delayAfterWave;
        _nextWaveTimerRoutine = StartCoroutine(NextWaveCountdown(delay));
    }

    private IEnumerator NextWaveCountdown(float delay)
    {
        float elapsed = 0f;

        // delay만큼 또는 버튼 클릭(강제 시작)까지
        while (elapsed < delay)
        {
            float remain = delay - elapsed;
            hudView.UpdateNextWaveTimer(remain);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 0초가 되면 뷰에도 0 표시
        hudView.UpdateNextWaveTimer(0f);
    }
}
