// StageUIManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageUIManager : MonoBehaviour
{
    private MainTowerController mainTowerController;

    [Header("HP Display")]
    public TextMeshProUGUI txtHP;

    [Header("Points Display")]
    public TextMeshProUGUI txtStagePoints;
    public TextMeshProUGUI txtGlobalPoints;

    [Header("Wave Display")]
    public TextMeshProUGUI txtWave;

    [Header("GameOver/Clear Panel")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI resultText;
    public Transform starContainer;  // 클리어 시 별 아이콘 보여줄 곳
    public GameObject starPrefab;     // 별 프리팹
    public TextMeshProUGUI txtReward;

    public void Initialize(MainTowerController mainTower, int startStagePoints)
    {
        mainTowerController = mainTower;

        // 남은 체력 메인타워 체력크기로 초기화
        txtHP.text = mainTower.MaxHp.ToString();

        // 현재 웨이브 웨이브 SO 데이터 정보로 초기화
        txtWave.text = $"{WaveManager.Instance.CurrentWaveIndex} / {WaveManager.Instance.waveDataSO.waves.Length}";

        mainTowerController.OnHpChanged += UpdateHp;
        WaveManager.Instance.OnWaveIdxChanged += UpdateWave;

        // Points
        UpdateStagePoints(startStagePoints);
        //UpdateGlobalPoints(startGlobalPoints);

        //gameOverPanel.SetActive(false);
    }

    public void UpdateHp(int curHP, int maxHP)
    {
        txtHP.text = maxHP.ToString();
    }

    public void UpdateWave(int curWave, int maxWave)
    {
        txtWave.text = $"{curWave} / {maxWave}";
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
