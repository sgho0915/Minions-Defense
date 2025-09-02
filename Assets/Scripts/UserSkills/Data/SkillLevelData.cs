// SkillLevelData.cs
using UnityEngine;

/// <summary>
/// 모든 스킬 레벨 데이터의 기반이 되는 추상 클래스
/// UI 표시 및 공통 로직에 필요한 최소한의 데이터 정의
/// </summary>
[System.Serializable]
public abstract class SkillLevelData
{
    [Header("공통")]
    public int level;                   // 스킬 레벨 (1~3)    
    public int upgradeCost;             // 강화 비용    
    public int executeCost;             // 스킬 시전 비용    
    public GameObject modelPrefab;      // 모델 Prefab (투사체 또는 트랩)    
    public float cooldown;              // 쿨다운(초)
    public Sprite skillIcon;            // 스킬 아이콘
    [TextArea] public string skillDesc;            // 스킬 설명
}
