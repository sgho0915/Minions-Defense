// TowerWorldSelectionController.cs
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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

        // 설치 모드 중엔 선택 금지
        if (TowerPlacementController.Instance != null && TowerPlacementController.Instance.IsPlacing)
            return;

        // 설치 직후 0.2초 정도 설치 타워에 대한 선택 방지 시간 부여
        if (Time.unscaledTime - TowerPlacementController.LastPlacementTime < 0.2f)
            return;

        // UI 위면 무시
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // 클릭/탭 릴리즈 감지
        bool released = false;
        Vector2 pos = default;

        var ts = Touchscreen.current;
        if (ts != null)
        {
            var t = ts.primaryTouch;
            if (t.press.wasReleasedThisFrame) { released = true; pos = t.position.ReadValue(); }
        }
        else if (Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            released = true; pos = Mouse.current.position.ReadValue();
        }

        if (!released) return;

        // 레이캐스트로 타워 선택
        var ray = cam.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out var hit, 1000f, towerMask, QueryTriggerInteraction.Ignore))
        {
            var ctrl = hit.collider.GetComponentInParent<TowerController>();
            if (ctrl != null) SelectTower(ctrl);
        }
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
