using UnityEngine;
using DG.Tweening;

/// <summary>
/// 모든 UI View의 기본 베이스 추상 클래스
/// Fade 기반 Show / Hide 전환 애니메이션 포함
/// protected: 선언 클래스와 자식 클래스에서 접근 가능하도록 허용
/// abstract: 자식 클래스에서 반드시 override 해야함
/// virtual: 자식 클래스에서 선택적으로 override 가능
/// </summary>
public abstract class UIView : MonoBehaviour
{
    [Header("Fade 옵션")]
    [SerializeField] protected float fadeDuration = 0.25f; // UIView를 상속받는 클래스에서만 접근 가능

    protected CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    /// <summary>
    /// View를 부드럽게 표시 (Fade In)
    /// </summary>
    public virtual void Show()
    {
        gameObject.SetActive(true);

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.DOFade(1, fadeDuration).SetEase(Ease.OutQuad).OnStart(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        });
    }

    /// <summary>
    /// View를 부드럽게 숨김 (Fade Out)
    /// </summary>
    public virtual void Hide()
    {
        canvasGroup.DOFade(0, fadeDuration).SetEase(Ease.InQuad).OnComplete(() =>
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        });
    }
}
