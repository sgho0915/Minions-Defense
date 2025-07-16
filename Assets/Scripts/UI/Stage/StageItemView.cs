// StageItemView.cs
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageItemView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtStageName;
    [SerializeField] private Image imgThumbnail;
    [SerializeField] private Image[] starIcons;  // 3개 Image를 배열로
    [SerializeField] private Button btnPlay;

    [Header("Star Colors")]
    [SerializeField] private Color activeStarColor = Color.white;
    [SerializeField] private Color inactiveStarColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);

    private int _stageIndex;

    /// <summary>
    /// 외부에서 이 한 메서드만 호출해서 세팅하세요.
    /// </summary>
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

        // 클릭 리스너 등록 (중복 방지)
        btnPlay.onClick.RemoveAllListeners();
        btnPlay.onClick.AddListener(OnClickPlay);
    }

    private void OnClickPlay()
    {
        // 씬 이름은 "Stage_{index}" 형식이어야 합니다.
        SceneManager.LoadScene($"Stage_{_stageIndex}");
    }
}
