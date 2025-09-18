// UIView.cs
using System;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 1. 모든 UI View의 기본 베이스 추상 클래스
/// 2. Fade 기반 Show / Hide 전환에 대한 기본적인 애니메이션 동작 기능을 포함하므로 인터페이스(메서드만 정의, 공통 변수 선언 불가)로 정의 불가
/// 3. protected 사용 : 선언 클래스와 자식 클래스에서만 fadeDuration과 canvasGroup 변수에 접근 가능하도록 허용
/// 4. abstract 사용 : 자식 클래스에서 반드시 override해서 구현하도록 해야함(이 부분에서 virtual과 차이 있음)
/// 5. virtual 사용: 자식 클래스에서 선택적으로 Awake, Show, Hide 메서드를 override 해서 확장 가능하도록 하기 위함
/// 6. UIView를 상속받는 View 클래스들에서 Awake, Show, Hide를 override 해 사용할 때 base.Awake(), base.Show(), base.Hide()가 포함되는 이유는 부모클래스에 정의된 해당 함수를 수행해 부모클래스 초기화 및 수행돼야할 기본 기능을 동작시켜 부모의 핵심기능을 재사용하면서 각 View의 개별 기능을 덧붙이기 위해 사용
/// </summary>

[RequireComponent (typeof(CanvasGroup))]    // canvasGroup이 없으면 자동 추가
public abstract class UIView : MonoBehaviour
{
    [Header("Fade 옵션")]
    [SerializeField] protected float fadeDuration = 0.25f;

    public CanvasGroup canvasGroup;
    private Tweener _fadeTween;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// View를 부드럽게 표시 (Fade In)
    /// </summary>
    public virtual void Show()
    {
        _fadeTween?.Kill();  // 이전 페이드 애니메이션 중지
        gameObject.SetActive(true);

        _fadeTween = canvasGroup.DOFade(1, fadeDuration)            
            .SetEase(Ease.OutQuad)  // Ease: 애니메이션의 속도 변화 곡선 설정                                    
            .OnStart(() =>  // OnStart: 트윈 애니메이션이 시작될 때 한 번 호출될 함수(콜백) 등록
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            })
            .SetUpdate(true)    // SetUpdate(true): Time.timeScale 값에 영향을 받지 않고 항상 정상 속도로 실행되도록 설정                                
            .OnKill(() => _fadeTween = null); // OnKill: 트윈이 종료될 때 호출될 콜백을 등록. 트윈이 끝나면 참조를 null로 만들어 메모리 누수를 방지하고 상태를 관리
    }


    /// <summary>
    /// View를 숨기기만 하고 아무런 콜백이 필요 없는 경우에 대한 오버로딩(이름 같고 파라미터 다름)
    /// </summary>
    public virtual void Hide()
    {
        Hide(null);
    }

    /// <summary>
    /// View를 숨긴 후 실행될 콜백이 필요한 경우
    /// </summary>
    /// <param name="onHideComplete">숨김 애니메이션 완료 후 실행될 콜백</param>
    public virtual void Hide(Action onHideComplete = null)
    {
        SoundManager.Instance.PlayButtonClicked();
        _fadeTween?.Kill();

        _fadeTween = canvasGroup.DOFade(0, fadeDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                gameObject.SetActive(false);
                onHideComplete?.Invoke();
            })
            .SetUpdate(true)
            .OnKill(() => _fadeTween = null);
    }
}
