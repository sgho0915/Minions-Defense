using DG.Tweening;
using LeTai.Asset.TranslucentImage;
using System;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StagePauseView : UIView
{
    [Header("Buttons")]
    [SerializeField] private Button btnResume;
    [SerializeField] private Button btnSettings;
    [SerializeField] private Button btnBackToLobby;

    public event Action OnHideComplete;
    public event Action OnBackToLobbyClicked;

    protected override void Awake()
    {
        base.Awake();   // 부모클래스인 UIView의 Awake 실행

        btnResume.onClick.RemoveAllListeners();
        btnResume.onClick.AddListener(Hide);

        btnBackToLobby.onClick.RemoveAllListeners();
        btnBackToLobby.onClick.AddListener(() => OnBackToLobbyClicked?.Invoke());
    }

    public override void Show()
    {
        Time.timeScale = 0; // 게임 시간 정지
        base.Show();        // 부모의 Show 로직 수행
    }

    public override void Hide()
    {
        base.Hide(() =>
        {
            Time.timeScale = 1;
            OnHideComplete?.Invoke();
        });
    }
}
