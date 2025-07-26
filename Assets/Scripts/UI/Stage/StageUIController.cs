// StageUIManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class StageUIController : MonoBehaviour
{
    [Header("View 참조 요소")]
    [SerializeField] private StageHUDView hudView;
    [SerializeField] private StageResultView resultView;
    
    private MainTowerController mainTower;
    private WaveManager waveManager;

    private void OnEnable()
    {
        resultView.Hide();
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
}
