// TowerInfoView.cs
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 선택된 타워 아이템의 데이터를 표시하는 View
/// </summary>
public class TowerInfoView : UIView
{
    [Header("타워 구매 View")]
    [SerializeField] private GameObject root;          // 뷰 루트 오브젝트
    [SerializeField] private Image imgIcon;            // 타워 아이콘
    [SerializeField] private TMP_Text txtName;         // 타워 이름
    [SerializeField] private TMP_Text txtDescription;  // 타워 설명
    [SerializeField] private TMP_Text txtLevel;        // 타워 레벨
    [SerializeField] private TMP_Text txtDamage;       // 타워 데미지
    [SerializeField] private TMP_Text txtAttackCycle;  // 타워 공격 주기
    [SerializeField] private TMP_Text txtAttackRange;  // 타워 공격 범위
    [SerializeField] private TMP_Text txtAoESpeed;     // 타워 투사체 속도
    [SerializeField] private TMP_Text txtAoERange;     // 타워 광역 공격 범위
    [SerializeField] private TMP_Text txtBuyPrice;     // 구매 가격
    [SerializeField] private Button btnBuy;            // 구매 버튼
    [SerializeField] private Button btnClose;          // 닫기

    [Header("타워 강화 & 판매 View")]
    [SerializeField] private GameObject rootUpgrade;
    [SerializeField] private Image imgCurrentIcon;         // 현재 타워 아이콘
    [SerializeField] private TMP_Text txtCurName;          // 현재 타워 이름
    [SerializeField] private TMP_Text txtCurDescription;   // 현재 타워 설명
    [SerializeField] private TMP_Text txtCurLevel;         // 현재 타워 레벨
    [SerializeField] private TMP_Text txtCurDamage;        // 현재 타워 데미지
    [SerializeField] private TMP_Text txtCurAttackCycle;   // 현재 타워 공격 주기
    [SerializeField] private TMP_Text txtCurAttackRange;   // 현재 타워 공격 범위
    [SerializeField] private TMP_Text txtCurAoESpeed;      // 현재 타워 투사체 속도
    [SerializeField] private TMP_Text txtCurAoERange;      // 현재 타워 광역 공격 범위
    [SerializeField] private TMP_Text txtNextLevel;        // 다음 레벨 타워 레벨
    [SerializeField] private TMP_Text txtNextDamage;       // 다음 레벨 타워 데미지
    [SerializeField] private TMP_Text txtNextAttackCycle;  // 다음 레벨 타워 공격 주기
    [SerializeField] private TMP_Text txtNextAttackRange;  // 다음 레벨 타워 공격 범위
    [SerializeField] private TMP_Text txtNextAoESpeed;     // 다음 레벨 타워 투사체 속도
    [SerializeField] private TMP_Text txtNextAoERange;     // 다음 레벨 타워 광역 공격 범위
    [SerializeField] private TMP_Text txtUpgradePrice;     // 강화 가격
    [SerializeField] private TMP_Text txtSellPrice;        // 판매 가격
    [SerializeField] private Button btnUpgrade;            // 강화 버튼
    [SerializeField] private Button btnSell;               // 판매 버튼
    [SerializeField] private Button btnCloseUpgrade;       // 닫기 버튼

    public event Action<TowerDataSO> OnBuyClicked;  // 타워 구매 이벤트
    public event Action OnUpgradeClicked;           // 타워 강화 이벤트
    public event Action OnSellClicked;              // 타워 판매 이벤트

    private TowerDataSO _currentTowerData;

    protected override void Awake()
    {
        base.Awake();

        btnClose.onClick.AddListener(() => Hide());
        btnCloseUpgrade.onClick.AddListener(() => Hide());
        btnBuy.onClick.AddListener(() => { if (_currentTowerData != null) OnBuyClicked?.Invoke(_currentTowerData); });
        btnUpgrade.onClick.AddListener(() => OnUpgradeClicked?.Invoke());
        btnSell.onClick.AddListener(() => OnSellClicked?.Invoke());
    }

    public void SetUpgradeInteractable(bool can) => btnUpgrade.interactable = can;

    /// <summary>
    /// 타워 구매 뷰 표시
    /// </summary>
    /// <param name="dataSO"></param>
    public void ShowTowerBuyView(TowerDataSO dataSO)
    {
        base.Show();
        rootUpgrade.SetActive(false);

        _currentTowerData = dataSO;

        var lv1Data = dataSO.levelData[0];

        root.SetActive(true);
        txtName.text = dataSO.towerName;
        imgIcon.sprite = lv1Data.levelIcon;
        txtDescription.text = lv1Data.description;
        txtLevel.text = $"레벨 {lv1Data.level}";
        txtDamage.text = lv1Data.damage.ToString("0.0");
        txtAttackCycle.text = lv1Data.attackSpeed.ToString("0.0");
        txtAttackRange.text = lv1Data.range.ToString("0.0");
        txtAoESpeed.text = lv1Data.projectileSpeed.ToString("0.0");
        txtAoERange.text = lv1Data.splashRadius.ToString("0.0");
        txtBuyPrice.text = $"-{lv1Data.upgradeCost}";
    }


    /// <summary>
    /// 타워 강화 / 판매 뷰
    /// </summary>
    public void ShowTowerUpgradeView(TowerDataSO towerSO, int currentLevelIdx)
    {
        base.Show();
        root.SetActive(false);

        if (towerSO == null)
        {
            Debug.LogError("[TowerInfoView] TowerDataSO is null. Make sure factory passes DataSO on Initialize.");
            Hide();
            return;
        }

        if (root) root.SetActive(false);
        if(rootUpgrade) rootUpgrade.SetActive(true);

        var cur = towerSO.levelData[currentLevelIdx]; // 현재 타워 레벨 정보
        var next = (currentLevelIdx + 1 < towerSO.levelData.Length) 
            ? towerSO.levelData[currentLevelIdx + 1] 
            : null; // 다음 레벨 타워 정보

        imgCurrentIcon.sprite = cur.levelIcon;
        txtCurName.text = towerSO.towerName;
        txtCurDescription.text = cur.description;
        txtCurLevel.text = $"레벨 {cur.level}";
        txtCurDamage.text = cur.damage.ToString("0.0");
        txtCurAttackCycle.text = cur.attackSpeed.ToString("0.0");
        txtCurAttackRange.text = cur.range.ToString("0.0");
        txtCurAoESpeed.text = cur.projectileSpeed.ToString("0.0");
        txtCurAoERange.text = cur.splashRadius.ToString("0.0");

        txtSellPrice.text = cur.sellPrice.ToString(); // 판매가는 현재 레벨 기준

        if(next != null)
        {
            txtNextLevel.text = $"레벨 {next.level}";
            txtNextDamage.text = next.damage.ToString("0.0");
            txtNextAttackCycle.text = next.attackSpeed.ToString("0.0");
            txtNextAttackRange.text = next.range.ToString("0.0");
            txtNextAoESpeed.text = next.projectileSpeed.ToString("0.0");
            txtNextAoERange.text = next.splashRadius.ToString("0.0");
            txtUpgradePrice.text = $"-{next.upgradeCost}";
            btnUpgrade.interactable = true ;
        }
        else
        {
            // 최대 레벨 UI
            txtNextLevel.text = "-";
            txtNextDamage.text = "-";
            txtNextAttackCycle.text = "-";
            txtNextAttackRange.text = "-";
            txtNextAoESpeed.text = "-";
            txtNextAoERange.text = "-";
            txtUpgradePrice.text = "MAX";
            btnUpgrade.interactable = false;
        }
    }


    /// <summary>
    /// View 숨기기
    /// </summary>
    public override void Hide()
    {
        base.Hide(() => {
            root.SetActive(false);
            rootUpgrade.SetActive(false);
        });
    }
}
