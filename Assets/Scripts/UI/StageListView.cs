using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �������� ����Ʈ View ����
/// </summary>
public class StageListView : UIView
{
    [SerializeField] private Button btnBackToLobby;

    private LobbyUIManager lobbyUIManager;

    protected override void Awake()
    {
        base.Awake();
        lobbyUIManager = FindObjectOfType<LobbyUIManager>();
        btnBackToLobby.onClick.AddListener(OnClickBackToLobby);
    }

    private void OnClickBackToLobby()
    {
        lobbyUIManager.OnClickLobbyButton();
    }
}
