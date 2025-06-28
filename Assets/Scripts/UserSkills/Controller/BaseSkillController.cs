// BaseSkillController.cs
using UnityEngine;
using System.Linq;

/// <summary>
/// 스킬 공통 로직 추상 클래스
/// - 공통 필드, 쿨다운, 레벨 관리
/// </summary>
[RequireComponent(typeof(AudioSource))]
public abstract class BaseSkillController<TLevelData> : MonoBehaviour, ISkill where TLevelData : class
{
    protected TLevelData[] _levels;
    protected int _curIdx;
    protected Transform _owner;
    protected AudioSource _audio;
    protected float _lastCastTime = -Mathf.Infinity;

    public virtual void Initialize(ScriptableObject dataSO, Transform owner)
    {
        _audio = GetComponent<AudioSource>();
        _owner = owner;
        switch (dataSO)
        {
            case MagicPoeDataSO mSO:
                _levels = mSO.levels as TLevelData[];
                break;
            case SpikeDataSO sSO:
                _levels = sSO.levels as TLevelData[];
                break;
        }
        SetLevel(1);
    }

    public virtual void SetLevel(int level)
    {
        _curIdx = _levels.ToList().FindIndex(x =>
        {
            var common = (x.GetType().GetField("common").GetValue(x) as CommonSkillLevelData);
            return common.level == level;
        });
    }

    public bool IsReady()
    {
        var common = GetCommon();
        return Time.time >= _lastCastTime + common.cooldown;
    }

    protected void RecordCast() => _lastCastTime = Time.time;

    public abstract void CastSkill(Vector3 targetPosition);

    protected CommonSkillLevelData GetCommon()
        => _levels[_curIdx].GetType()
            .GetField("common")
            .GetValue(_levels[_curIdx]) as CommonSkillLevelData;
}