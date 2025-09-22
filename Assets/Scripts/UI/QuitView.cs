// QuitView.cs
using UnityEngine;
using UnityEngine.UI;

public class QuitView : UIView
{
    [Header("Buttons")]
    [SerializeField] private Button quitButton;
    [SerializeField] private Button cancelButton;

    private LobbyUIManager lobbyUIManager;

    protected override void Awake()
    {
        base.Awake();
        lobbyUIManager = FindObjectOfType<LobbyUIManager>();

        quitButton.onClick.AddListener(OnClickQuitGame);
        cancelButton.onClick.AddListener(OnClickBackToLobby);
    }


    private void OnClickQuitGame()
    {
        SoundManager.Instance.PlayButtonClicked();
        Application.Quit();
    }

    private void OnClickBackToLobby()
    {
        lobbyUIManager.OnClickLobbyButton();
    }
}
