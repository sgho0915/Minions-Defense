// TowerSkillSelectionController.cs
using UnityEngine;

/// <summary>
/// TowerListView와 TowerInfoView를 연결하는 Controller
/// </summary>
public class TowerSkillSelectionController : MonoBehaviour
{
    [Header("Views")]
    [SerializeField] TowerSkillListView listView;
    [SerializeField] TowerInfoView towerInfoView;
    [SerializeField] SkillInfoView skillInfoView;

    [Header("Data")]
    [SerializeField] TowerDataSO[] allTowers;
    [SerializeField] SkillDataSO[] allSkills;

    private System.Collections.Generic.List<ISkill> _activeSkills = new();  // 생성된 스킬 컨트롤러 인스턴스 저장

    private void Awake()
    {
        // 타워 이벤트 구독
        listView.OnTowerSelected += HandleTowerSelected;
        towerInfoView.OnBuyClicked += HandleTowerBuy;

        // 스킬 이벤트 구독
        listView.OnSkillSelected += HandleSkillSelected;
        skillInfoView.OnExecuteClicked += HandleSkillExecute;
        skillInfoView.OnUpgradeClicked += HandleSkillUpgrade;
    }

    private void Start()
    {
        listView.PopulateTowers(allTowers);

        foreach (var skillSO in allSkills)
        {
            ISkill skillInstance = skillSO.CreateSkill(this.transform);
            _activeSkills.Add(skillInstance);
        }
        listView.PopulateSkills(_activeSkills);

        towerInfoView.Hide();
        skillInfoView.Hide();
    }

    private void OnDestroy()
    {
        listView.OnTowerSelected -= HandleTowerSelected;
        towerInfoView.OnBuyClicked -= HandleTowerBuy;

        listView.OnSkillSelected -= HandleSkillSelected;
        skillInfoView.OnExecuteClicked -= HandleSkillExecute;
        skillInfoView.OnUpgradeClicked -= HandleSkillUpgrade;
    }

    /// <summary>
    /// 리스트에서 타워를 선택 시 상세정보 TowerInfoView에 반영
    /// </summary>
    /// <param name="towerDataSO"></param>
    private void HandleTowerSelected(TowerDataSO towerDataSO)
    {
        towerInfoView.ShowTowerBuyView(towerDataSO);
        skillInfoView.Hide();
    }

    private void HandleTowerBuy(TowerDataSO towerDataSO)
    {
        // 레벨1 기준으로 배치 시작
        TowerPlacementController.Instance.BeginPlacement(towerDataSO, towerDataSO.levelData[0]);
        towerInfoView.Hide();
    }

    private void HandleSkillSelected(ISkill skill)
    {
        towerInfoView.Hide();
        skillInfoView.Show(skill);
    }

    private void HandleSkillExecute(ISkill skill)
    {
        int cost = skill.CurrentLevelData.executeCost;
        if (GameManager.Instance.TrySpendStagePoints(cost))
        {
            skill.ExecuteSkill(Vector3.zero);
            skillInfoView.Hide();
        }
        else
        {
            Debug.Log("스테이지 포인트 부족");
        }
    }

    private void HandleSkillUpgrade(ISkill skill)
    {
        // 다음레벨 데이터 확인, 비용차감, skill.setlevel 호출 등 강화 로직
    }
}
