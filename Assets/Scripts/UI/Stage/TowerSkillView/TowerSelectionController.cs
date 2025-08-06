// TowerSelectionController.cs
using Unity.VisualScripting;
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
    }

    private void Start()
    {
        listView.Populate(allTowers);
        infoView.Hide();
    }

    private void OnDestroy()
    {
        listView.OnTowerSelected -= HandleTowerSelected;
    }

    /// <summary>
    /// 리스트에서 타워를 선택 시 상세정보 TowerInfoView에 반영
    /// </summary>
    /// <param name="dataSO"></param>
    private void HandleTowerSelected(TowerDataSO dataSO)
    {
        infoView.ShowLv1Info(dataSO);
    }
}
