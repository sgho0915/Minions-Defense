// StageListView.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 스테이지 리스트 View 로직
/// </summary>
public class StageListView : UIView
{
    [SerializeField] private Button btnBackToLobby;

    [Header("Prefab & Container")]
    [SerializeField] private StageItemView stageItemPrefab;
    [SerializeField] private Transform stageListContent;    // StageList ScrollView -> Viewport -> Content

    [Header("스테이지별 메타데이터")]
    [SerializeField] private StageData[] stageDatas;

    private LobbyUIManager lobbyUIManager;

    protected override void Awake()
    {
        base.Awake();
        lobbyUIManager = FindObjectOfType<LobbyUIManager>();
        btnBackToLobby.onClick.AddListener(OnClickBackToLobby);

        // 각 데이터마다 버튼을 Instantiate + Setup
        foreach (var data in stageDatas)
        {
            // 저장된 값이 없으면 0을 디폴트로
            data.starGrade = PlayerPrefs.GetInt($"Stage_{data.stageIndex}_Stars", 0);

            var item = Instantiate(stageItemPrefab, stageListContent);
            item.Setup(data);
        }
    }

    private void OnClickBackToLobby()
    {
        lobbyUIManager.OnClickLobbyButton();
    }
}
