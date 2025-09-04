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
        backgroundImageSource.BlurConfig.Strength = 0;  // �� �� 0���� �ʱ�ȭ

        Time.timeScale = 0; // ���� �ð� ����

        DOTween.To(
            () => backgroundImageSource.BlurConfig.Strength,      // ���� �� (���� �����̴� ��)
            x => backgroundImageSource.BlurConfig.Strength = x,   // �� ���� (�� ������ �����̴� �� ����)
            10,                     // ��ǥ ��
            1f                       // �ɸ��� �ð�
        ).SetUpdate(true);
    }

    public void Hide()
    {
        rootPanel.SetActive(false);
        Time.timeScale = 1; // ���� �ð� �ٽ� �帧
    }
}
