// SpikeController.cs
using UnityEngine;

/// <summary>
/// “스파이크” 스킬 컨트롤러
/// </summary>
public class SpikeController : BaseSkillController<SpikeLevelData>
{
    public override void CastSkill(Vector3 targetPosition)
    {
        if (!IsReady()) return;

        var lvl = _levels[_curIdx];
        var common = lvl.commonSkillData;

        // 트랩 설치
        var trap = Instantiate(common.modelPrefab, targetPosition, Quaternion.identity);
        var comp = trap.AddComponent<SpikeTrap>();
        comp.Setup(lvl);

        RecordCast();
    }
}
