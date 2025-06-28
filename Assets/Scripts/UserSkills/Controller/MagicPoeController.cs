// MagicPoeController.cs
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

/// <summary>
/// “마법의 포댕이” 스킬 컨트롤러
/// </summary>
public class MagicPoeController : BaseSkillController<MagicPoeLevelData>
{
    public override void CastSkill(Vector3 targetPosition)
    {
        if (!IsReady()) return;

        var lvl = _levels[_curIdx];
        var common = lvl.commonSkillData;

        // 1) 투사체 인스턴스화
        var proj = Instantiate(common.modelPrefab, _owner.position, Quaternion.identity);
        // 2) 방향 설정
        Vector3 dir = (targetPosition - _owner.position).normalized;
        proj.transform.rotation = Quaternion.LookRotation(dir);

        // 3) 애니메이션 재생
        var anim = proj.GetComponent<Animator>();
        if (anim != null && lvl.moveAnim != null)
            anim.Play(lvl.moveAnim.name);

        // 4) 투사체 로직 부착
        var mover = proj.AddComponent<MagicPoeProjectile>();
        mover.Setup(lvl);

        RecordCast();
    }
}
