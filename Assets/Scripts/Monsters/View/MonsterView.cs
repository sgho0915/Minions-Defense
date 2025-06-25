// MonsterView.cs
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// MonsterHealthModel 이벤트 구독 → Slider/UI 갱신(View)
/// </summary>
public class MonsterHealthView : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;
    [SerializeField] private CanvasGroup canvas;

    private MonsterHealthModel _model;

    public void Initialize(MonsterHealthModel model)
    {
        _model = model;
        hpSlider.maxValue = model.MaxHp;
        hpSlider.value = model.CurrentHp;
        canvas.alpha = 1f;

        model.OnHpChanged += UpdateHpBar;
        model.OnDied += HideHpBar;
    }

    private void UpdateHpBar(int cur, int max) => hpSlider.value = cur;
    private void HideHpBar() => canvas.alpha = 0f;

    private void OnDestroy()
    {
        if (_model != null)
        {
            _model.OnHpChanged -= UpdateHpBar;
            _model.OnDied -= HideHpBar;
        }
    }
}
