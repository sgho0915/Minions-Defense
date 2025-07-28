// StageResultView.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 게임 종료 시 결과 패널 전담 View
/// - 평가 기준별 별색 변경
/// - 클리어/실패 배경 블러 색상 변경
/// - 버튼 표시 제어
/// </summary>
public class StageResultView : MonoBehaviour
{
    [Header("Root Panel")]
    [SerializeField] private GameObject rootPanel;

    [Header("Background Blur")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Color successColor = new Color(1f, 1f, 1f, 0.6f);
    [SerializeField] private Color failColor = new Color(0.7f, 0f, 0f, 0.6f);

    [Header("Result Text & Stars")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Image[] starIcons;
    [SerializeField] private Color activeStarColor = Color.yellow;
    [SerializeField] private Color inactiveStarColor = Color.gray;

    [Header("Buttons")]
    [SerializeField] private Button btnRetry;  
    [SerializeField] private Button btnBack;   

    /// <summary>
    /// 결과 패널 열기
    /// </summary>
    /// <param name="clear">클리어 여부</param>
    /// <param name="criteriaMet">평가기준 충족 여부 배열(길이 3)</param>
    public void ShowResult(bool clear, bool[] criteriaMet)
    {
        rootPanel.SetActive(true);
        backgroundImage.color = clear ? successColor : failColor;
        resultText.text = clear ? "Stage Clear!" : "Game Over";

        for (int i = 0; i < starIcons.Length; i++)
        {
            bool met = (i < criteriaMet.Length && criteriaMet[i]);
            starIcons[i].color = met ? activeStarColor : inactiveStarColor;
        }

        btnRetry.gameObject.SetActive(!clear);
        btnBack.gameObject.SetActive(!clear);
    }

    /// <summary>
    /// 결과 패널 닫기
    /// </summary>
    public void Hide()
    {
        rootPanel.SetActive(false);
    }
}
