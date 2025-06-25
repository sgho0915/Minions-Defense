// MonsterModel.cs
using System;

/// <summary>
/// 런타임 체력 데이터와 변경 이벤트 발행(Model)
/// </summary>
public class MonsterHealthModel
{
    public int MaxHp { get; }
    public int CurrentHp { get; private set; }

    public event Action<int, int> OnHpChanged;
    public event Action OnDied;

    public MonsterHealthModel(int maxHp)
    {
        MaxHp = maxHp;
        CurrentHp = maxHp;
    }

    public void TakeDamage(int dmg)
    {
        CurrentHp = Math.Max(0, CurrentHp - dmg);
        OnHpChanged?.Invoke(CurrentHp, MaxHp);
        if (CurrentHp == 0) OnDied?.Invoke();
    }
}
