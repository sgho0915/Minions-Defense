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
    [SerializeField] private Button button;
    [SerializeField] private Image imgCooldownOverlay;
    [SerializeField] private TMP_Text txtCooldown;

    private ISkill _skill;
    float _skillTime = 0;

    // 어떤 스킬을 표시하고, 클릭했을 때 어떤 행동을 할지 외부에서 주입
    public void Setup(ISkill skill, Action<ISkill> onClick)
    {
        _skill = skill;

        imgIcon.sprite = _skill.CurrentLevelData.skillIcon;

        button.onClick.AddListener(() => onClick(skill));
    }

    public void UpdateView()
    {
        if(_skill != null)
            imgIcon.sprite = _skill.CurrentLevelData.skillIcon;
    }

    // 매 프레임 쿨다운 상태를 체크하여 UI에 반영
    private void Update()
    {
        if (_skill != null && imgCooldownOverlay != null)
        {
            _skillTime = _skill.GetCooldownTime() / _skill.CurrentLevelData.cooldown;
            txtCooldown.text = _skill.GetCooldownTime().ToString("F1");

            imgCooldownOverlay.gameObject.SetActive(!_skill.IsReady());
            imgCooldownOverlay.fillAmount = _skillTime;

            button.interactable = _skill.IsReady();
        }
    }
}
