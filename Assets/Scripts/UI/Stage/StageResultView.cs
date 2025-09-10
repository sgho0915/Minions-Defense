// StageResultView.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using LeTai.Asset.TranslucentImage;
using DG.Tweening;
using System;

/// <summary>
/// 게임 종료 시 결과 패널 전담 View
/// - 평가 기준별 별색 변경
/// - 클리어/실패 배경 블러 색상 변경
/// - 버튼 표시 제어
/// </summary>
public class StageResultView : UIView
{
    [Header("Root Panel")]
    [SerializeField] private GameObject rootPanel;

    [Header("Background Blur")]
    [SerializeField] private TranslucentImage backgroundImage;
    [SerializeField] private TranslucentImageSource backgroundImageSource;
    [SerializeField] private Color successColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] private Color failColor = new Color(1f, 0.5f, 0.5f, 1f);

    [Header("Result Text & Stars & Reward")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Image[] starIcons;
    [SerializeField] private Color activeStarColor = Color.yellow;
    [SerializeField] private Color inactiveStarColor = Color.gray;
    [SerializeField] private TextMeshProUGUI rewardText;

    [Header("Buttons")]
    [SerializeField] private Button btnRetry;  
    [SerializeField] private Button btnBack;

    public event Action OnBackToLobbyClicked;

    protected override void Awake()
    {
        base.Awake();

        btnRetry.onClick.RemoveAllListeners();
        btnBack.onClick.RemoveAllListeners();

        btnRetry.onClick.AddListener(RestryStage);
        btnBack.onClick.AddListener(() => OnBackToLobbyClicked?.Invoke());
    }

    /// <summary>
    /// 결과 패널 열기
    /// </summary>
    /// <param name="clear">클리어 여부</param>
    /// <param name="criteriaMet">평가기준 충족 여부 배열(길이 3)</param>
    public void ShowResult(bool clear, bool[] criteriaMet, int reward)
    {
        Time.timeScale = 0;
        base.Show();

        rootPanel.SetActive(true);
        backgroundImage.color = clear ? successColor : failColor;
        backgroundImageSource.BlurConfig.Strength = 0;  // 블러 값 0으로 초기화

        DOTween.To(
            () => backgroundImageSource.BlurConfig.Strength,      // 시작 값 (현재 슬라이더 값)
            x => backgroundImageSource.BlurConfig.Strength = x,   // 값 적용 (매 프레임 슬라이더 값 변경)
            10,                     // 목표 값
            1f                       // 걸리는 시간
        ).SetUpdate(true);

        resultText.text = clear ? "Stage Clear!" : "Game Over";

        for (int i = 0; i < starIcons.Length; i++)
        {
            bool met = (i < criteriaMet.Length && criteriaMet[i]);
            starIcons[i].color = met ? activeStarColor : inactiveStarColor;
        }

        rewardText.text = $"+{reward.ToString()}";
    }

    /// <summary>
    /// 결과 패널 닫기
    /// </summary>
    public override void Hide()
    {
        base.Hide(() =>
        {
            rootPanel.SetActive(false);
        });       
    }


    public void RestryStage()
    {         
        SceneManager.LoadScene($"Stage_{GameManager.Instance.stageIndex}"); // 현재 스테이지를 다시 시작
    }
}
