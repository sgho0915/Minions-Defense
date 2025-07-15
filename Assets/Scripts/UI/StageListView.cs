using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 스테이지 리스트 View 로직
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

    public void OnClickPlayStage(int stageIndex)
    {
        SceneManager.LoadScene($"Stage_{stageIndex}");
    }
}
