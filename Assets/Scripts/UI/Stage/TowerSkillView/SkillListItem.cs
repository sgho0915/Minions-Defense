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
    [SerializeField] private TMP_Text txtDesc;
    [SerializeField] private TMP_Text txtCost;
    [SerializeField] private Button btnExecute;
    [SerializeField] private Button btnUpgrade;

    public void Setup(MagicPoeDataSO data, Action onExecuteClick, Action onUpgradeClick)
    {
        btnExecute.onClick.AddListener(() => onExecuteClick());
        btnUpgrade.onClick.AddListener(() => onUpgradeClick());
    }
}
