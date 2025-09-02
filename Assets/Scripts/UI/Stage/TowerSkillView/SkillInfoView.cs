// SkillInfoView.cs
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfoView : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private GameObject root;
    [SerializeField] private Image imgIcon;
    [SerializeField] private TMP_Text txtName;
    [SerializeField] private TMP_Text txtDescription;
    [SerializeField] private TMP_Text txtLevel;
    [SerializeField] private TMP_Text txtUpgradeCost;
    [SerializeField] private TMP_Text txtExecuteCost;
    [SerializeField] private TMP_Text txtDamage;
    [SerializeField] private TMP_Text txtCooldown;
    [SerializeField] private TMP_Text txtRange;
    [SerializeField] private TMP_Text txtMaxDamage;

    [SerializeField] private TMP_Text txtNextLevel;
    [SerializeField] private TMP_Text txtNextDamage;
    [SerializeField] private TMP_Text txtNextCooldown;
    [SerializeField] private TMP_Text txtNextRange;
    [SerializeField] private TMP_Text txtNextMaxDamage;

    [SerializeField] private Button btnExecute;
    [SerializeField] private Button btnUpgrade;
    [SerializeField] private Button btnClose;

    public event Action<ISkill> OnExecuteClicked;
    public event Action<ISkill> OnUpgradeClicked;

    private ISkill _currentSkill;
    private bool _isMaxlevel;

    private void Awake()
    {
        btnClose.onClick.AddListener(Hide);
        btnExecute.onClick.AddListener(() => {
            if (_currentSkill != null) OnExecuteClicked?.Invoke(_currentSkill);
        });
        btnUpgrade.onClick.AddListener(() => {
            if (_currentSkill != null) OnUpgradeClicked?.Invoke(_currentSkill);
        });
    }

    // Update에서 버튼 상태를 계속 갱신하여 쿨다운, 자원 부족 시 비활성화
    private void Update()
    {
        if (root.activeSelf && _currentSkill != null)
        {
            bool canAfford = GameManager.Instance.stagePoints >= _currentSkill.CurrentLevelData.executeCost;
            btnExecute.interactable = _currentSkill.IsReady() && canAfford;
            btnUpgrade.interactable = canAfford && !_isMaxlevel;
        }
    }

    public void Show(ISkill skill)
    {
        _currentSkill = skill;
        if (_currentSkill == null)
        {
            Hide();
            return;
        }

        var _currentLevelData = _currentSkill.CurrentLevelData;
        var nextLevelData = _currentSkill.NextLevelData;
        var _currentSkillDataSO = _currentSkill.CurrentSkillDataSO;

        root.SetActive(true);

        txtName.text = _currentSkillDataSO.skillName;
        imgIcon.sprite = _currentLevelData.skillIcon;
        txtDescription.text = _currentLevelData.skillDesc;
        txtExecuteCost.text = _currentLevelData.executeCost.ToString();

        txtLevel.text = $"레벨 {_currentLevelData.level}";
        txtCooldown.text = $"{_currentLevelData.cooldown}초";

        

        // 스킬 타입에 따라 다른 정보 표시
        if (_currentLevelData is MagicPoeLevelData poeData)
        {
            txtDamage.text = poeData.damagePerTick.ToString();
            txtRange.text = poeData.auraRadius.ToString("F1");
            txtMaxDamage.text = poeData.maxDamageOutput.ToString();
            txtUpgradeCost.text = poeData.upgradeCost.ToString();

            if(nextLevelData != null && nextLevelData is MagicPoeLevelData nextPoeData)
            {
                txtNextLevel.text = $"레벨 {nextLevelData.level}";
                txtNextCooldown.text = $"{nextLevelData.cooldown}초";

                txtNextDamage.text = nextPoeData.damagePerTick.ToString();
                txtNextRange.text = nextPoeData.auraRadius.ToString("F1");
                txtNextMaxDamage.text = nextPoeData.maxDamageOutput.ToString();
                _isMaxlevel = false;
            }
            else
            {
                txtNextLevel.text = "-";
                txtNextCooldown.text = "-";
                txtNextDamage.text = "-";
                txtNextRange.text = "-";
                txtNextMaxDamage.text = "-";
                txtUpgradeCost.text = "MAX";
                _isMaxlevel = true;
            }
        }
        // else if (data is SpikeLevelData spikeData) { ... }
    }

    public void ShowUpgradeView()
    {        
        if (_currentSkill == null)
        {
            Debug.LogError("[SkillinfoView] ISkill is null");
            return;
        }

        var curDataSO = _currentSkill.CurrentSkillDataSO;

        

        if (curDataSO is MagicPoeDataSO magicPoeDataSO)
        {
            var curLevelDataSO = magicPoeDataSO.levels[_currentSkill.CurrentLevel - 1];
            var nextLevelDataSO = (_currentSkill.CurrentLevel < magicPoeDataSO.levels.Length)
                ? magicPoeDataSO.levels[_currentSkill.CurrentLevel]
                : null;

            imgIcon.sprite = curLevelDataSO.skillIcon;
            txtDescription.text = curLevelDataSO.skillDesc;
            txtExecuteCost.text = curLevelDataSO.executeCost.ToString();

            txtLevel.text = $"레벨 {curLevelDataSO.level}";
            txtCooldown.text = $"{curLevelDataSO.cooldown}초";
            txtDamage.text = curLevelDataSO.damagePerTick.ToString();
            txtRange.text = curLevelDataSO.auraRadius.ToString("F1");
            txtMaxDamage.text = curLevelDataSO.maxDamageOutput.ToString();
            txtUpgradeCost.text = curLevelDataSO.upgradeCost.ToString();

            if (nextLevelDataSO != null)
            {
                txtNextLevel.text = $"레벨 {nextLevelDataSO.level}";
                txtNextCooldown.text = $"{nextLevelDataSO.cooldown}초";
                txtNextDamage.text = nextLevelDataSO.damagePerTick.ToString();
                txtNextRange.text = nextLevelDataSO.auraRadius.ToString("F1");
                txtNextMaxDamage.text = nextLevelDataSO.maxDamageOutput.ToString();
            }
            else
            {
                txtNextLevel.text = "-";
                txtNextCooldown.text = "-";
                txtNextDamage.text = "-";
                txtNextRange.text = "-";
                txtNextMaxDamage.text = "-";
                txtUpgradeCost.text = "-";
            }
        }
    }

    public void Hide()
    {
        _currentSkill = null;
        root.SetActive(false);
    }
}
