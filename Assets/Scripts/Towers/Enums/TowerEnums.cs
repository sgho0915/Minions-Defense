// TowerEnums.cs
/// <summary>
/// 타워 구분용 타입 열거형
/// </summary>
public enum TowerType
{
    Mage, Cannon, MachineGun, Main
}

/// <summary>
/// 타겟 우선순위 옵션
/// </summary>
public enum TowerTargetPriority
{
    First, Last, Strongest, Weakest, Closest
}

/// <summary>
/// AoE 광역 공격 효과 형태
/// </summary>
public enum TowerAreaShape
{
    None, Circle, Cone, Line
}