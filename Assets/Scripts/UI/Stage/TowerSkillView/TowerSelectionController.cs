// TowerSelectionController.cs
using UnityEngine;

/// <summary>
/// TowerListView와 TowerInfoView를 연결하는 Controller
/// </summary>
public class TowerSelectionController : MonoBehaviour
{
    [SerializeField] TowerListView listView;
    [SerializeField] TowerInfoView infoView;
    [SerializeField] TowerDataSO[] allTowers;

    private void Awake()
    {
        listView.OnTowerSelected += HandleTowerSelected;
        infoView.OnBuyClicked += HandleTowerBuy;
    }

    private void Start()
    {
        listView.Populate(allTowers);
        infoView.Hide();
    }

    private void OnDestroy()
    {
        listView.OnTowerSelected -= HandleTowerSelected;
        infoView.OnBuyClicked -= HandleTowerBuy;
    }

    /// <summary>
    /// 리스트에서 타워를 선택 시 상세정보 TowerInfoView에 반영
    /// </summary>
    /// <param name="towerDataSO"></param>
    private void HandleTowerSelected(TowerDataSO towerDataSO)
    {
        infoView.ShowLv1Info(towerDataSO);
    }

    private void HandleTowerBuy(TowerDataSO towerDataSO)
    {
        // 레벨1 기준으로 배치 시작
        TowerPlacementController.Instance.BeginPlacement(towerDataSO, towerDataSO.levelData[0]);
        infoView.Hide();
    }
}
