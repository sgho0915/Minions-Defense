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
    [SerializeField] private TMP_Text txtExecuteCost;
    [SerializeField] private TMP_Text txtCooldown;
    [SerializeField] private TMP_Text txtDamage;
    [SerializeField] private Button btnExecute;
    [SerializeField] private Button btnUpgrade;
    [SerializeField] private Button btnClose;

    public event Action<ISkill> OnExecuteClicked;
    public event Action<ISkill> OnUpgradeClicked;

    private ISkill _currentSkill;

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

        var data = _currentSkill.CurrentLevelData;
        var skillSO = (skill as MonoBehaviour)?.GetComponent<MagicPoeController>()?
            .GetType().GetField("_data", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(skill) as SkillDataSO;


        root.SetActive(true);

        txtName.text = skillSO.skillName;
        imgIcon.sprite = data.skillIcon;
        txtDescription.text = data.skillDesc;
        txtLevel.text = $"레벨 {data.level}";
        txtExecuteCost.text = $"비용: {data.executeCost}";
        txtCooldown.text = $"쿨다운: {data.cooldown:F1}초";

        // 스킬 타입에 따라 다른 정보 표시 (예시)
        if (data is MagicPoeLevelData poeData)
        {
            txtDamage.text = $"틱당 피해량: {poeData.damagePerTick}";
        }
        // else if (data is SpikeLevelData spikeData) { ... }
    }

    public void Hide()
    {
        _currentSkill = null;
        root.SetActive(false);
    }
}
