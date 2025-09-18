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

    [SerializeField] Transform skillControllersParent;

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
            ISkill skillInstance = skillSO.CreateSkill(skillControllersParent);
            _activeSkills.Add(skillInstance);
        }
        listView.PopulateSkills(_activeSkills);
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
        SoundManager.Instance.PlayButtonClicked();
        if(skillInfoView.gameObject.activeSelf) skillInfoView.Hide();
        towerInfoView.ShowTowerBuyView(towerDataSO);
    }

    private void HandleTowerBuy(TowerDataSO towerDataSO)
    {
        SoundManager.Instance.PlayButtonClicked();
        // 레벨1 기준으로 배치 시작
        TowerPlacementController.Instance.BeginPlacement(towerDataSO, towerDataSO.levelData[0]);
        towerInfoView.Hide();
    }

    private void HandleSkillSelected(ISkill skill)
    {
        SoundManager.Instance.PlayButtonClicked();
        if (towerInfoView.gameObject.activeSelf) towerInfoView.Hide();
        skillInfoView.Show(skill);
    }

    private void HandleSkillExecute(ISkill skill)
    {
        SoundManager.Instance.PlayButtonClicked();
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
        SoundManager.Instance.PlayButtonClicked();
        // 현재 스킬 다음 레벨 데이터 불러오기
        var skillDataSO = skill.CurrentSkillDataSO;
        var skillLevelData = skill.CurrentLevelData;

        // GameManager에 스테이지 포인트 차감 요청
        int cost = skillLevelData.upgradeCost;
        if (GameManager.Instance.TrySpendStagePoints(cost))
        {
            // 다음 레벨 스킬 데이터로 SkillInfoView, 하단 스킬 아이콘 업데이트 명령
            if (skillLevelData is MagicPoeLevelData nextPoeLevelData)
            {
                // 스킬 Controller 레벨 업
                var controller = (skill as MonoBehaviour)?.GetComponent<MagicPoeController>();
                controller.SetLevel(skillLevelData.level + 1);

                // SkillInfoView UI 업데이트
                //skillInfoView.ShowUpgradeView();
                skillInfoView.Show(skill);

                // 스킬 아이콘 업데이트
                listView.RefreshSkillItemView(skill);

                Debug.Log($"{skillDataSO.skillName} : 레벨{controller.CurrentLevelData.level} 업그레이드 완료");
            }
        }
        else
        {
            Debug.Log("스테이지 포인트 부족");
        }
    }
}
