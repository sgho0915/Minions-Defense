// TowerInfoView.cs
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 선택된 타워 아이템의 데이터를 표시하는 View
/// </summary>
public class TowerInfoView : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private Image imgIcon;
    [SerializeField] private TMP_Text txtName;
    [SerializeField] private TMP_Text txtDescription;
    [SerializeField] private TMP_Text txtBuyPrice;
    [SerializeField] private TMP_Text txtSellPrice;
    [SerializeField] private Button btnBuy;
    [SerializeField] private Button btnSell;
    [SerializeField] private Button btnUpgrade;
    [SerializeField] private Button btnClose;

    public event Action<TowerDataSO> OnBuyClicked;  // 타워 구매 : 컨트롤러에 타워 배치모드 전환 알림
    private TowerDataSO _currentTowerData;

    private void Awake()
    {
        btnClose.onClick.AddListener(() => Hide());
        btnBuy.onClick.AddListener(() => { if (_currentTowerData != null) OnBuyClicked?.Invoke(_currentTowerData); });
    }

    /// <summary>
    /// 리스트에서 선택했을 경우 Lv1 타워 데이터를 기준으로 UI에 표시
    /// </summary>
    /// <param name="dataSO"></param>
    public void ShowLv1Info(TowerDataSO dataSO)
    {
        _currentTowerData = dataSO;

        var lv1Data = dataSO.levelData[0];

        root.SetActive(true);
        txtName.text = dataSO.towerName;
        imgIcon.sprite = lv1Data.levelIcon;
        txtDescription.text = lv1Data.description;
        txtBuyPrice.text = lv1Data.upgradeCost.ToString();
        txtSellPrice.text = lv1Data.sellPrice.ToString();
    }

    public void Hide() => root.SetActive(false);
}
