// LobbyView.cs
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Lobby 캔버스 내 UI 요소 관련 View 및 버튼 핸들링
/// </summary>
public class LobbyView : UIView
{
    [SerializeField] private Button btnStageList;

    private LobbyUIManager lobbyUIManager;

    protected override void Awake()
    {
        base.Awake();
        lobbyUIManager = FindObjectOfType<LobbyUIManager>();

        btnStageList.onClick.AddListener(OnClickStageList);
    }

    private void OnClickStageList()
    {
        lobbyUIManager.OnClickStageButton();
    }
}
