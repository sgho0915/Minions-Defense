// StageUIManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageUIManager : MonoBehaviour
{
    [Header("HP Slider")]
    public Slider hpSlider;

    [Header("Points Display")]
    public TextMeshProUGUI txtStagePoints;
    public TextMeshProUGUI txtGlobalPoints;

    [Header("GameOver/Clear Panel")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI resultText;
    public Transform starContainer;  // 클리어 시 별 아이콘 보여줄 곳
    public GameObject starPrefab;     // 별 프리팹
    public TextMeshProUGUI txtReward;

    public void Initialize(int maxHp, int startStagePoints, int startGlobalPoints)
    {
        // HP
        hpSlider.maxValue = maxHp;
        hpSlider.value = maxHp;

        // Points
        UpdateStagePoints(startStagePoints);
        UpdateGlobalPoints(startGlobalPoints);

        gameOverPanel.SetActive(false);
    }

    public void UpdateHp(int hp)
    {
        hpSlider.value = hp;
    }

    public void UpdateStagePoints(int pts)
    {
        txtStagePoints.text = pts.ToString();
    }

    public void UpdateGlobalPoints(int pts)
    {
        txtGlobalPoints.text = pts.ToString();
    }

    /// <summary>
    /// clear==true → 클리어, false → 실패
    /// </summary>
    public void ShowGameOver(bool clear, int stars = 0, int reward = 0)
    {
        gameOverPanel.SetActive(true);
        resultText.text = clear ? "Stage Clear!" : "Game Over";

        // Clear일 때만 별/보상 표시
        starContainer.gameObject.SetActive(clear);
        txtReward.gameObject.SetActive(clear);

        if (clear)
        {
            // 별 표시
            foreach (Transform c in starContainer) Destroy(c.gameObject);
            for (int i = 0; i < stars; i++)
                Instantiate(starPrefab, starContainer);

            txtReward.text = $"+{reward} pts";
        }
    }
}
