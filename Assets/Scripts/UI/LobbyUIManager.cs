using System.Collections;
using UnityEngine;

/// <summary>
/// Lobby Scene 내 UIView 간의 전환을 제어하는 UI 매니저
/// </summary>
public class LobbyUIManager : MonoBehaviour
{
    [Header("UIView Panels")]
    [SerializeField] private UIView lobbyView;      // 메인 로비 Canvas
    [SerializeField] private UIView stageListView;  // 스테이지 목록 Canvas

    

    private IEnumerator Start()
    {
        // 초기 상태 설정: 로비로 시작
        yield return null;
        lobbyView.Show();
        stageListView.Hide();
    }

    /// <summary>
    /// 스테이지 버튼 클릭: Lobby -> StageList 이동
    /// </summary>
    public void OnClickStageButton()
    {
        lobbyView.Hide();
        stageListView.Show();
    }

    public void OnClickLobbyButton()
    {
        lobbyView.Show();
        stageListView.Hide();
    }
}
