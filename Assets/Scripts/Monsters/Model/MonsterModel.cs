// MonsterModel.cs
using System;

/// <summary>
/// 런타임 체력 데이터와 변경 이벤트 발행(Model)
/// </summary>
public class MonsterModel
{
    public int MaxHp { get; }
    public int CurrentHp { get; private set; }
    public float CurrentMoveSpeed { get; private set; }


    public event Action<int, int> OnHpChanged;
    public event Action OnDied;

    public MonsterModel(int maxHp, float moveSpeed)
    {
        MaxHp = maxHp;
        CurrentHp = maxHp;
        CurrentMoveSpeed = moveSpeed;
    }

    public void TakeDamage(int dmg)
    {
        CurrentHp = Math.Max(0, CurrentHp - dmg);
        OnHpChanged?.Invoke(CurrentHp, MaxHp);
        if (CurrentHp == 0) OnDied?.Invoke();
    }

    public void Slow(float slowAmount, float slowDuration)
    {
        // 슬로우 구현 필요
    }
}
