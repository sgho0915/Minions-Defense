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
    [Header("UIView Panels")]
    [SerializeField] private UIView lobbyView;      // 메인 로비 Canvas
    [SerializeField] private UIView stageListView;  // 스테이지 목록 Canvas

    private List<UIView> _allViews;
    private UIView _currentView;

    private void Awake()
    {
        _allViews = new List<UIView> { lobbyView, stageListView };
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
        ShowView(stageListView);
    }

    public void OnClickLobbyButton()
    {
        ShowView(lobbyView);
    }
}
