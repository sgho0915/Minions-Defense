// SkillDataSO.cs
using UnityEngine;

/// <summary>
/// 모든 스킬 데이터 SO의 기반이 되는 추상 클래스
/// UI 표시에 필요한 공통 정보, 자신을 제어할 Controller를 생성하는 책임을 가짐
/// 추상 메서드를 가지고 있으므로 SkillDataSO도 추상 클래스가 됨
/// </summary>
public abstract class SkillDataSO : ScriptableObject
{
    [Header("기본 정보")]
    public string skillName;

    /// <summary>
    /// 해당 스킬 데이터를 제어할 스킬 컨트롤러를 생성하고 반환함
    /// </summary>
    /// <param name="owner">스킬 주체</param>
    /// <returns>생성된 스킬 컨트롤러</returns>
    public abstract ISkill CreateSkill(Transform owner);
}
