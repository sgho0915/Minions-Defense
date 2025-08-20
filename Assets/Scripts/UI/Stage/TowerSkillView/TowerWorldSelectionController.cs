// TowerWorldSelectionController.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// 월드의 타워를 선택하고, 강화/판매 UI를 표시/연동하는 컨트롤러
/// 입력 감지/선택/UI연동만 담당
public class TowerWorldSelectionController : MonoBehaviour
{
    [SerializeField] private Camera cam;               // 비우면 자동으로 Camera.main
    [SerializeField] private LayerMask towerMask;      // Tower가 있는 레이어
    [SerializeField] private TowerInfoView infoView;   // 강화/판매 UI

    private TowerController selected;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        // 버튼 콜백 연결
        infoView.OnUpgradeClicked += HandleUpgradeClicked;
        infoView.OnSellClicked += HandleSellClicked;
        // 스테이지 포인트 변동 시 업그레이드 가능 여부 버튼 상태 갱신
        GameManager.Instance.OnStagePointsChanged += OnPointsChanged;
    }

    void OnDestroy()
    {
        infoView.OnUpgradeClicked -= HandleUpgradeClicked;
        infoView.OnSellClicked -= HandleSellClicked;
        if (GameManager.Instance != null)
            GameManager.Instance.OnStagePointsChanged -= OnPointsChanged;

        UnsubscribeTowerEvents();
    }

    void Update()
    {
        if (cam == null) return;
        if (TowerPlacementController.Instance != null && TowerPlacementController.Instance.IsPlacing) return;
        if (Time.unscaledTime - TowerPlacementController.LastPlacementTime < 0.2f) return;

        //  마우스/터치 릴리즈 이벤트 동시 체크, 실제 발생한 입력만 채택
        Vector2 pointerPos;
        bool released = TryGetPointerRelease(out pointerPos);
        if (!released) return;

        // UI 위면 무시: 포인터 위치 기반 RaycastAll
        if (IsPointerOverUI(pointerPos)) return;

        // 레이캐스트로 타워 선택
        var ray = cam.ScreenPointToRay(pointerPos);
        if (Physics.Raycast(ray, out var hit, 1000f, towerMask, QueryTriggerInteraction.Collide))
        {
            var ctrl = hit.collider.GetComponentInParent<TowerController>();
            if (ctrl != null) SelectTower(ctrl);
        }
    }

    /// <summary>마우스 좌클릭 릴리즈 또는 터치 릴리즈가 발생했는지 검사하고 좌표를 반환</summary>
    private bool TryGetPointerRelease(out Vector2 pos) 
    {
        pos = default;

        var mouse = Mouse.current;
        var touch = Touchscreen.current;

        bool mouseReleased = mouse != null && mouse.leftButton.wasReleasedThisFrame; 
        bool touchReleased = false;

        if (touch != null)
        {
            var t = touch.primaryTouch;
            // 실제 터치 릴리즈만 인정 (디바이스 존재만으로는 true 금지)
            touchReleased = t.press.wasReleasedThisFrame;
        }

        if (mouseReleased)
        {
            pos = mouse.position.ReadValue();
            return true;
        }
        if (touchReleased)
        {
            pos = touch.primaryTouch.position.ReadValue();
            return true;
        }
        return false;
    }

    /// <summary>포인터 화면좌표 기준으로 UI가 가로막는지 검사</summary>
    private bool IsPointerOverUI(Vector2 screenPos)
    {
        if (EventSystem.current == null) return false;

        var data = new PointerEventData(EventSystem.current) { position = screenPos };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        return results.Count > 0;
    }

    void SelectTower(TowerController ctrl)
    {
        if (selected == ctrl)
        {
            // 이미 열려있으면 토글하려면 여기서 닫을 수도 있음
        }

        UnsubscribeTowerEvents();

        selected = ctrl;
        // 업그레이드로 본체가 교체될 때 UI가 새 컨트롤러로 이어지도록
        selected.OnTowerUpgraded += OnTowerUpgraded;

        // 강화뷰 열기
        int curIndex = Mathf.Max(0, selected.CurrentLevelIndex - 1);
        infoView.ShowTowerUpgradeView(selected.TowerDataSO, curIndex);

        // 현재 포인트 기준으로 버튼 상태 반영
        OnPointsChanged(GameManager.Instance.stagePoints);
    }

    void UnsubscribeTowerEvents()
    {
        if (selected != null)
        {
            selected.OnTowerUpgraded -= OnTowerUpgraded;
            selected = null;
        }
    }

    void OnTowerUpgraded(TowerController oldOne, TowerController newOne)
    {
        // 새 타워를 곧바로 선택 상태로 이어받기
        SelectTower(newOne);
    }

    void HandleUpgradeClicked()
    {
        if (selected == null) return;
        // 타워 내부가 비용 체크와 레벨 체크를 모두 수행
        selected.TryUpgrade();
        // TryUpgrade 성공 시 OnTowerUpgraded로 UI가 갱신됨
    }

    void HandleSellClicked()
    {
        if (selected == null) return;

        int refund = selected.Sell();
        GameManager.Instance.TryGiveStagePoints(refund);  // 환급
        infoView.Hide();
        UnsubscribeTowerEvents();
    }

    void OnPointsChanged(int newPoints)
    {
        if (selected == null) return;

        // 업그레이드 가능 여부를 버튼 interacatable로 반영
        if (selected.CanUpgrade(out var next, out var cost))
        {
            // bool affordable = newPoints >= cost;

            infoView.SetUpgradeInteractable(newPoints >= cost);
        }
    }
}
