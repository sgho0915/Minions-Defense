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

    [Header("Display Remain Time Until Next Wave Start")]
    [SerializeField] private Button btnForceStartWave;    
    [SerializeField] private TextMeshProUGUI txtCanvasNextWave;

    [Header("Points Display")]
    [SerializeField] private TextMeshProUGUI txtStagePoints;

    public event Action OnForceStartWaveButtonClicked; // Controller 버튼 클릭 구독용 이벤트 노출

    private void Awake()
    {
        // 클릭이 들어오면 다음 웨이브를 강제로 시작하도록 StageUiController로 이벤트만 날려줌
        btnForceStartWave.onClick.AddListener(() =>  OnForceStartWaveButtonClicked.Invoke());
    }

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

    /// <summary>
    /// 남은 초 단위로 표시
    /// </summary>
    public void UpdateNextWaveTimer(float remainSeconds)
    {
        txtCanvasNextWave.text = $"{remainSeconds:F1}s";
    }

    /// <summary>
    /// “강제 웨이브 시작” 버튼을 활성/비활성화
    /// </summary>
    public void SetNextWaveTimeUIInteractable(bool interactable, bool islastWave)
    {
        btnForceStartWave.interactable = interactable;

        if (islastWave) {
            btnForceStartWave.gameObject.SetActive(!islastWave);
            txtCanvasNextWave.gameObject.SetActive(!islastWave);
        }
    }
}
