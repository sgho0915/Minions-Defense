// LobbyUIManager.cs
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lobby Scene 내 UIView 간의 전환을 제어하는 UI 매니저
/// </summary>
public class LobbyUIManager : MonoBehaviour
{
    [Header("로비 View 요소")]
    [SerializeField] private UIView lobbyView;     
    [SerializeField] private UIView stageListView; 
    [SerializeField] private UIView quitView; 

    private List<UIView> _allViews;
    private UIView _currentView;

    private void Awake()
    {
        _allViews = new List<UIView> { lobbyView, stageListView, quitView };
    }

    private void Start()
    {
        foreach (var view in _allViews) { view.gameObject.SetActive(false); }

        ShowView(lobbyView);
    }

    /// <summary>
    /// 특정 View만 표시하고 나머지는 숨김
    /// </summary>
    private void ShowView(UIView viewToShow)
    {
        if (_currentView != null && _currentView != viewToShow)
            _currentView.Hide();

        viewToShow.Show();
        _currentView = viewToShow;
    }

    /// <summary>
    /// 스테이지 버튼 클릭: Lobby -> StageList 이동
    /// </summary>
    public void OnClickStageButton()
    {
        SoundManager.Instance.PlayButtonClicked();
        ShowView(stageListView);
    }

    public void OnClickLobbyButton()
    {
        SoundManager.Instance.PlayButtonClicked();
        ShowView(lobbyView);
    }

    public void OnClickSettingsButton()
    {
        SoundManager.Instance.PlayButtonClicked();
        GlobalUIManager.Instance.ShowSettingsView();
    }

    public void OnClickQuitButton()
    {
        SoundManager.Instance.PlayButtonClicked();
        ShowView(quitView);
    }
}
