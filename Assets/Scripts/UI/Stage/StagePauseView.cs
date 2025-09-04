using DG.Tweening;
using LeTai.Asset.TranslucentImage;
using UnityEngine;
using UnityEngine.UI;

public class StagePauseView : MonoBehaviour
{
    [Header("Root Panel")]
    [SerializeField] private GameObject rootPanel;

    [Header("Background Blur")]
    [SerializeField] private TranslucentImage backgroundImage;
    [SerializeField] private TranslucentImageSource backgroundImageSource;

    [Header("Buttons")]
    [SerializeField] private Button btnResume;
    [SerializeField] private Button btnSettings;
    [SerializeField] private Button btnBackToLobby;

    private void Awake()
    {
        btnResume.onClick.RemoveAllListeners();
        btnResume.onClick.AddListener(Hide);
    }

    public void Show()
    {
        rootPanel.SetActive(true);
        backgroundImageSource.BlurConfig.Strength = 0;  // 블러 값 0으로 초기화

        Time.timeScale = 0; // 게임 시간 멈춤

        DOTween.To(
            () => backgroundImageSource.BlurConfig.Strength,      // 시작 값 (현재 슬라이더 값)
            x => backgroundImageSource.BlurConfig.Strength = x,   // 값 적용 (매 프레임 슬라이더 값 변경)
            10,                     // 목표 값
            1f                       // 걸리는 시간
        ).SetUpdate(true);
    }

    public void Hide()
    {
        rootPanel.SetActive(false);
        Time.timeScale = 1; // 게임 시간 다시 흐름
    }
}
