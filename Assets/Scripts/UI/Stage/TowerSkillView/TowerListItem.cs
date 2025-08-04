// TowerListItem.cs
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 타워 리스트의 항목 View
/// </summary>
public class TowerListItem : MonoBehaviour
{
    [SerializeField] private Image imgIcon;
    [SerializeField] private TMP_Text txtName;
    [SerializeField] private TMP_Text txtCost;
    [SerializeField] private Button button;

    /// <summary>
    /// 선택한 타워 데이터로 세부 정보 UI 세팅 및 클릭 콜백 등록
    /// </summary>
    /// <param name="data">타워 레벨 데이터</param>
    /// <param name="onClick">클릭 콜백</param>
    public void Setup(TowerDataSO data, Action onClick)
    {
        // 레벨1에 해당하는 아이콘, 이름만 표시
        imgIcon.sprite = data.levelData[0].levelIcon;
        txtName.text = data.towerName;
        txtCost.text = data.levelData[0].upgradeCost.ToString();

        button.onClick.AddListener(() =>  onClick());
    }
}
