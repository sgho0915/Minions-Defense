// ISkill.cs
using UnityEngine;

/// <summary>
/// 스킬 동작 계약 인터페이스
/// </summary>
public interface ISkill
{
    /// <summary>데이터와 주체(owner)로 초기화, 어떤 스킬 데이터 SO든 받을 수 있음</summary>
    void Initialize(SkillDataSO dataSO, Transform owner);

    /// <summary>레벨 설정 (1~3)</summary>
    void SetLevel(int level);

    /// <summary>스킬 시전 (타겟 위치)</summary>
    void ExecuteSkill(Vector3 targetPosition);

    /// <summary>쿨다운 가능 여부</summary>
    bool IsReady();

    /// <summary>남은 쿨타임</summary>
    float GetCooldownTime();

    // UI 업데이트를 위한 프로퍼티
    int CurrentLevel { get; }
    SkillDataSO CurrentSkillDataSO { get; }
    SkillLevelData CurrentLevelData { get; }
    SkillLevelData NextLevelData { get; }
}
