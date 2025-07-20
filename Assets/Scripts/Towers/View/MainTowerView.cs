// MainTowerView.cs
using DG.Tweening;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// MainTowerController 전용 View
/// </summary>
public class MainTowerView : MonoBehaviour
{
    [SerializeField] Slider hpSlider;
    private CanvasGroup canvasGroup;
    private MainTowerController mainTowerController;

    public void Initialize(MainTowerController mainTower)
    {
        mainTowerController = mainTower;
        canvasGroup = GetComponent<CanvasGroup>();
        hpSlider.maxValue = mainTowerController.MaxHp;
        hpSlider.value = mainTowerController.CurrentHp;
        canvasGroup.alpha = 1;

        mainTowerController.OnHpChanged += UpdateHpBar;
        mainTowerController.OnDied += HideHpBar;
    }

    private void UpdateHpBar(int curHp, int max)
    {
        hpSlider.value = curHp;
        Debug.Log($"메인타워 최대체력:{max}, 현재체력:{curHp}");
    }

    private void HideHpBar()
    {
        canvasGroup.DOFade(0, 3);
    }

    private void OnDestroy()
    {
        if (mainTowerController != null)
        {
            mainTowerController.OnHpChanged -= UpdateHpBar;
            mainTowerController.OnDied -= HideHpBar;
        }
    }
}
