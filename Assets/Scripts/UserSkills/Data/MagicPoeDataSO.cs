// MagicPoeDataSO.cs
using UnityEngine;

[CreateAssetMenu(fileName = "MagicPoeData", menuName = "TowerDefenseAssets/Skill/MagicPoeDataSO")]
/// <summary>
/// 마법의 포댕이 스킬 전체 레벨 데이터를 보관하는 SO
/// </summary>
public class MagicPoeDataSO : ScriptableObject
{
    [Header("기본 정보")]
    public string skillName;
    public Sprite icon;
    [Header("레벨별 설정 (1~3)")]
    public MagicPoeLevelData[] levels;
}
