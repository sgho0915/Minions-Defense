// SkillListItem.cs
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스킬 리스트 항목 View
/// </summary>
public class SkillListItem : MonoBehaviour
{
    [SerializeField] private Image imgIcon;
    [SerializeField] private TMP_Text txtName;
    [SerializeField] private TMP_Text txtCost;
    [SerializeField] private Button button;
    [SerializeField] private Image imgCooldownOverlay;

    private ISkill _skill;

    // 어떤 스킬을 표시하고, 클릭했을 때 어떤 행동을 할지 외부에서 주입
    public void Setup(ISkill skill, Action<ISkill> onClick)
    {
        _skill = skill;
        var data = skill.CurrentLevelData;
        var skillSO = (skill as MonoBehaviour)?.GetComponent<MagicPoeController>()?
            .GetType().GetField("_data", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(skill) as SkillDataSO;

        txtName.text = skillSO.skillName;
        imgIcon.sprite = data.skillIcon;
        txtCost.text = data.executeCost.ToString();

        button.onClick.AddListener(() => onClick(skill));
    }

    // 매 프레임 쿨다운 상태를 체크하여 UI에 반영
    private void Update()
    {
        if (_skill != null && imgCooldownOverlay != null)
        {
            imgCooldownOverlay.gameObject.SetActive(!_skill.IsReady());
            button.interactable = _skill.IsReady();
        }
    }
}
