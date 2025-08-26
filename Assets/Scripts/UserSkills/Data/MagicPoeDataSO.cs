// MagicPoeDataSO.cs
using UnityEngine;

[CreateAssetMenu(fileName = "MagicPoeData", menuName = "TowerDefenseAssets/Skill/MagicPoeDataSO")]
public class MagicPoeDataSO : SkillDataSO // SkillDataSO 상속
{
    [Header("레벨별 설정 (1~3)")]
    public MagicPoeLevelData[] levels;

    /// <summary>
    /// 마법의 포댕이 스킬 컨트롤러를 생성하고 초기화
    /// </summary>
    public override ISkill CreateSkill(Transform owner)
    {
        GameObject go = new GameObject(skillName + "_Controller");
        var controller = go.AddComponent<MagicPoeController>();
        controller.Initialize(this, owner); // 'this'를 넘겨서 자신을 초기화
        return controller;
    }
}