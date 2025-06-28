// SkillFactory.cs
using UnityEngine;

/// <summary>
/// 스킬 컨트롤러 생성 전담 싱글톤 팩토리
/// </summary>
public class SkillFactory
{
    private static SkillFactory _instance;
    public static SkillFactory Instance => _instance ??= new SkillFactory();
    private SkillFactory() { }

    /// <summary>
    /// ScriptableObject 타입에 맞춰 컨트롤러 생성 및 초기화
    /// </summary>
    public ISkill CreateSkill(ScriptableObject dataSO, Transform owner)
    {
        GameObject go = new GameObject(dataSO.name + "_Skill");
        ISkill ctrl;
        if (dataSO is MagicPoeDataSO magicPoeSO)
        {
            ctrl = go.AddComponent<MagicPoeController>();
            ctrl.Initialize(magicPoeSO, owner);
        }
        else if (dataSO is SpikeDataSO spikeSO)
        {
            ctrl = go.AddComponent<SpikeController>();
            ctrl.Initialize(spikeSO, owner);
        }
        else
        {
            GameObject.Destroy(go);
            throw new System.ArgumentException("Unknown SkillDataSO type");
        }
        return ctrl;
    }
}
