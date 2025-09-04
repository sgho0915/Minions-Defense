// StageHUDView.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

/// <summary>
/// 스테이지 플레이 중 HUD(View) 요소 관리
/// - HP 바, 웨이브, 스테이지 포인트 텍스트 업데이트
/// </summary>
public class StageHUDView : MonoBehaviour
{
    [Header("HP Display")]
    [SerializeField] private TextMeshProUGUI txtHP;

    [Header("Wave Display")]
    [SerializeField] private TextMeshProUGUI txtCurrentWave;    

    [Header("Points Display")]
    [SerializeField] private TextMeshProUGUI txtStagePoints;

    [Header("Pause Button")]
    [SerializeField] private Button btnPause;
    public event Action OnPauseClicked;

    /// <summary>
    /// HP 바 텍스트 갱신
    /// </summary>
    public void UpdateHp(int curHp, int maxHp)
    {
        txtHP.text = $"{curHp} / {maxHp}";
    }

    /// <summary>
    /// 웨이브 텍스트 갱신
    /// </summary>
    public void UpdateWave(int curWave, int maxWave)
    {
        txtCurrentWave.text = $"{curWave} / {maxWave}";
    }

    /// <summary>
    /// 스테이지 포인트 텍스트 갱신
    /// </summary>
    public void UpdateStagePoints(int pts)
    {
        txtStagePoints.text = pts.ToString();
    }

    public void StagePause()
    {
        btnPause.onClick.RemoveAllListeners();
        btnPause.onClick.AddListener(() => OnPauseClicked?.Invoke());
    }
}
