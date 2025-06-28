// ISkill.cs
using UnityEngine;

/// <summary>
/// 스킬 동작 계약 인터페이스
/// </summary>
public interface ISkill
{
    /// <summary>데이터와 주체(owner)로 초기화</summary>
    void Initialize(ScriptableObject dataSO, Transform owner);

    /// <summary>레벨 설정 (1~3)</summary>
    void SetLevel(int level);

    /// <summary>스킬 시전 (타겟 위치)</summary>
    void CastSkill(Vector3 targetPosition);

    /// <summary>쿨다운 가능 여부</summary>
    bool IsReady();
}
