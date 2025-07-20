// MainTowerController.cs
using System;
using UnityEngine;

public class MainTowerController : MonoBehaviour
{
    public event Action<int, int> OnHpChanged;
    public event Action OnDied;
    public int MaxHp { get; private set; }
    public int CurrentHp { get; private set; }

    [Header("View")]
    [SerializeField] private MainTowerView view;

    private void Start()
    {
        Initialize(1000);
    }

    public void Initialize(int hp)
    {
        MaxHp = hp;
        CurrentHp = hp;
        view.Initialize(this);
    }

    public void ApplyDamage(int dmg)
    {
        CurrentHp = Mathf.Max(0, CurrentHp - dmg);
        OnHpChanged?.Invoke(CurrentHp, MaxHp);
        if (CurrentHp == 0) OnDied?.Invoke();
    }
}
