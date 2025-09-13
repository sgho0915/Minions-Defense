// StageItemView.cs
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageItemView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtStageName;
    [SerializeField] private Image imgThumbnail;
    [SerializeField] private Image[] starIcons; 
    [SerializeField] private Button btnPlay;

    [Header("Star Colors")]
    [SerializeField] private Color activeStarColor = Color.white;
    [SerializeField] private Color inactiveStarColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);

    private int _stageIndex;

    public void Setup(StageData data)
    {
        _stageIndex = data.stageIndex;
        txtStageName.text = data.stageName;
        imgThumbnail.sprite = data.stageThumbnail;

        // 별 아이콘들 활성화
        for (int i = 0; i < starIcons.Length; i++)
        {
            if (i < data.starGrade)
                starIcons[i].color = activeStarColor;
            else
                starIcons[i].color = inactiveStarColor;
        }

        btnPlay.onClick.RemoveAllListeners();
        btnPlay.onClick.AddListener(OnClickPlay);
    }

    private void OnClickPlay()
    {
        SceneManager.LoadScene($"Stage_{_stageIndex}");
    }
}
