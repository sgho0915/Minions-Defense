// SpikeDataSO.cs
using UnityEngine;

[CreateAssetMenu(fileName = "SpikeData", menuName = "TowerDefenseAssets/Skill/SpikeDataSO")]
/// <summary>
/// 스파이크 스킬 전체 레벨 데이터를 보관하는 SO
/// </summary>
public class SpikeDataSO : ScriptableObject
{
    [Header("기본 정보")]
    public string skillName;
    public Sprite icon;
    [Header("레벨별 설정 (1~3)")]
    public SpikeLevelData[] levels;
}
