// CommonSkillLevelData.cs
using UnityEngine;

[System.Serializable]
/// <summary>
/// 스킬 레벨의 공통 필드(레벨, 쿨타임, 프리팹 등)
/// </summary>
public class CommonSkillLevelData
{    
    public int level;                   // 스킬 레벨 (1~3)    
    public int upgradeCost;             // 강화 비용    
    public GameObject modelPrefab;      // 모델 Prefab (투사체 또는 트랩)    
    public float cooldown;              // 쿨다운(초)
    public Sprite skillIcon;            // 스킬 아이콘
    public string skillDesc;            // 스킬 설명
}
