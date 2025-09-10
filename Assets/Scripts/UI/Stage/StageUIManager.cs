// StageUIManager.cs
using NUnit.Framework;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageUIManager : MonoBehaviour
{
    [Header("스테이지 View 요소")]
    [SerializeField] private TowerSkillListView towerSkillListView;
    [SerializeField] private TowerInfoView towerInfoView;
    [SerializeField] private SkillInfoView skillInfoView;
    [SerializeField] private StageHUDView hudView;
    [SerializeField] private StagePauseView pauseView;
    [SerializeField] private StageResultView resultView;

    private List<UIView> _backgroundViews;

    private void Awake()
    {
        _backgroundViews = new List<UIView> { towerSkillListView, hudView };

        // 이벤트 구독
        hudView.OnPauseClicked += ShowPauseView;    // HUDView에 속한 일시정지 버튼 누르면 PauseView 활성화
        pauseView.OnHideComplete += ShowBackgroundViews;    // 일시정지 -> 게임재개 시 배경 View 활성화

        pauseView.OnBackToLobbyClicked += BackToLobby;
        resultView.OnBackToLobbyClicked += BackToLobby;

    }

    private void OnDestroy()
    {
        // 이벤트 구독 해지
        hudView.OnPauseClicked -= ShowPauseView;
        pauseView.OnHideComplete -= ShowBackgroundViews;
        pauseView.OnBackToLobbyClicked -= BackToLobby;
        resultView.OnBackToLobbyClicked -= BackToLobby;
    }

    public void ShowResultView(bool clear, bool[] criteriaMet, int reward)
    {
        HideBackgroundViews();
        resultView.ShowResult(clear, criteriaMet, reward);
    }

    private void ShowPauseView()
    {
        HideBackgroundViews();
        pauseView.Show();
    }

    private void HideBackgroundViews()
    {
        foreach(var view in _backgroundViews) view.Hide();
    }

    private void ShowBackgroundViews()
    {
        foreach (var view in _backgroundViews) view.Show();
    }

    private void BackToLobby()
    {
        Time.timeScale = 1;
        GameManager.Instance.CleanupStageScene();
        SceneManager.LoadScene(0);    }

}
