using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TowerPlacementController : MonoBehaviour
{
    public static TowerPlacementController Instance { get; private set; }

    [Header("카메라(설치 모드 제어)")]
    [SerializeField] private CameraController cameraController;
    //[SerializeField] private float edgeThresholdPx = 60f;
    [SerializeField] private float edgePanStrength = 2.0f;

    [Header("Ray Probe")]
    public float rayMaxDistance = 5000f;   // 포인터 방향으로 이만큼 날아가서 아래로 다운캐스트

    [Header("레이어")]
    public LayerMask groundMask;    // 바닥(표면 포인트용)
    public LayerMask buildableMask; // 배치 가능
    public LayerMask blockedMask;   // 길/바위/기존타워 등 금지

    [Header("표면 추적(프리뷰 이동) 마스크")]
    [SerializeField] private LayerMask followSurfaceMask = ~0; // ★★★ 프리뷰가 '따라갈' 표면. 기본 Everything

    [Header("프리뷰/스냅")]
    public Material previewMat;
    public float footprintRadius = 0.6f;
    public bool snapToGrid = true;
    public float cellSize = 1f;

    [Header("색상")]
    public Color validColor = new(0, 1, 0, 0.45f);
    public Color invalidColor = new(1, 0, 0, 0.45f);

    [Header("에디터 키(테스트)")]
    public Key confirmKey = Key.Enter;
    public Key cancelKey = Key.Escape;

    [Header("엣지팬 인식 범위(퍼센트/세이프에어리어)")]
    [SerializeField] private bool useSafeAreaForEdge = true;     // 노치/제스처 영역 제외
    [SerializeField, Range(0f, 0.3f)] private float edgePctX = 0.08f; // 가로의 8%
    [SerializeField, Range(0f, 0.3f)] private float edgePctY = 0.08f; // 세로의 8%
    [SerializeField] private float minEdgePx = 40f;              // 너무 작은 폰 보호
    [SerializeField] private float mobileEdgePanStrengthMul = 1.8f; // 모바일 보정(체감 속도 ↑)
    // 가장자리에서 멀어질수록 가속도를 얼마나 주는지 (1=선형, 2=더 부드럽게)
    [SerializeField] private float edgePanResponseExponent = 1.2f;

    [Header("패드 중앙 스냅(부드럽게)")]
    [SerializeField] private float snapSmoothTime = 0.12f;   // 패드 위에서 중앙으로 안착
    [SerializeField] private float unsnapSmoothTime = 0.06f; // 패드 밖에서 포인터 추적
    [SerializeField] private string anchorChildName = "Anchor"; // 패드에 이 이름의 자식이 있으면 그 위치로 스냅(선택)
    [SerializeField] private float anchorYOffset = 0f;          // 앵커 기준 Y 오프셋(선택)
    
    Vector3 smoothVel; // SmoothDamp 속도 버퍼

    Camera cam;
    GameObject previewRoot;
    Renderer[] previewRenderers;
    MaterialPropertyBlock mpb;
    RangeIndicator rangeIndicator;

    TowerDataSO dataSO;
    TowerLevelData lv;
    int price;
    bool isPlacing, isValid;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        cam = Camera.main;
        mpb = new MaterialPropertyBlock();
    }

    public void BeginPlacement(TowerDataSO so, TowerLevelData level)
    {
        CancelPlacement();

        dataSO = so;
        lv = level;
        price = level.upgradeCost;

        previewRoot = Instantiate(level.towerPrefab);

        // ★ 프리뷰는 레이캐스트에 안 걸리게 (자기 자신 히트 방지)
        previewRoot.layer = LayerMask.NameToLayer("Ignore Raycast");
        foreach (var c in previewRoot.GetComponentsInChildren<Collider>(true))
            c.enabled = false;

        previewRenderers = previewRoot.GetComponentsInChildren<Renderer>(true);
        if (previewMat != null)
            foreach (var r in previewRenderers) r.sharedMaterial = previewMat;

        rangeIndicator = previewRoot.GetComponent<RangeIndicator>() ?? previewRoot.AddComponent<RangeIndicator>();
        rangeIndicator.SetRadius(level.range);

        isPlacing = true;
        SetPreviewColor(invalidColor);
        rangeIndicator.SetColor(invalidColor);
        smoothVel = Vector3.zero; // 타워 배치가능 영역 스무딩 버퍼 초기화

        if (cameraController != null) cameraController.SetUserPan(false);
    }


    void Update()
    {
        if (!isPlacing) return;

        if (TryGetPointer(out var screenPos, out bool released, out int pointerId))
        {
            bool overUI = false;
            if (EventSystem.current != null)
            {
                if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
                    overUI = EventSystem.current.IsPointerOverGameObject(pointerId); // 터치
                else
                    overUI = EventSystem.current.IsPointerOverGameObject();          // 마우스
            }

            UpdatePreviewPoseAndValidity(screenPos);
            EdgePanIfNearScreenBorder();

            if (!overUI && (released || (Keyboard.current != null && Keyboard.current[confirmKey].wasPressedThisFrame)))
                TryPlace();

            if (Keyboard.current != null && Keyboard.current[cancelKey].wasPressedThisFrame)
                CancelPlacement();
        }
    }

    bool TryGetPointer(out Vector2 pos, out bool released, out int pointerId)
    {
        released = false;
        pointerId = -1;

        var ts = Touchscreen.current;
        if (ts != null)
        {
            var touch = ts.primaryTouch;
            if (touch.press.isPressed || touch.press.wasReleasedThisFrame || touch.press.wasPressedThisFrame)
            {
                pos = touch.position.ReadValue();
                released = touch.press.wasReleasedThisFrame;
                pointerId = touch.touchId.ReadValue();
                return true;
            }
        }

        var mouse = Mouse.current;
        if (mouse != null)
        {
            pos = mouse.position.ReadValue();
            released = mouse.leftButton.wasReleasedThisFrame;
            pointerId = -1;
            return true;
        }

        pos = default;
        return false;
    }

    void UpdatePreviewPoseAndValidity(Vector2 screenPos)
    {
        int surfaceMask = followSurfaceMask.value;
        var ray = cam.ScreenPointToRay(screenPos);

        // 1) 프리뷰가 '따라갈' 표면 포인트(어디든 OK)
        if (!TryGetSurfacePoint(ray, surfaceMask, out var followPoint))
        {
            isValid = false;
            return;
        }

        // 2) 패드 위면 중앙 앵커로, 아니면 포인터 지점으로
        Vector3 targetPos;
        bool overBuildable = TryGetBuildableAnchor(followPoint, out targetPos);

        // 패드 밖일 땐(선택) 그리드 스냅 + 높이 보정
        if (!overBuildable && snapToGrid)
        {
            targetPos.x = Mathf.Round(targetPos.x / cellSize) * cellSize;
            targetPos.z = Mathf.Round(targetPos.z / cellSize) * cellSize;

            if (Physics.Raycast(targetPos + Vector3.up * 20f, Vector3.down,
                out var snapHit, 100f, surfaceMask, QueryTriggerInteraction.Collide))
            {
                targetPos.y = snapHit.point.y;
            }
        }

        // 3) 부드러운 이동 (패드 위/밖 각각 다른 smoothTime)
        float smoothTime = overBuildable ? snapSmoothTime : unsnapSmoothTime;
        previewRoot.transform.position =
            Vector3.SmoothDamp(previewRoot.transform.position, targetPos, ref smoothVel, smoothTime);

        // 4) 설치 가능 판정은 '현재 프리뷰 위치' 기준
        Vector3 p = previewRoot.transform.position;
        bool onBuildable = Physics.Raycast(p + Vector3.up * 0.5f, Vector3.down, out _, 2f, buildableMask.value);
        bool noOverlap = !Physics.CheckSphere(p + Vector3.up * 0.2f, footprintRadius, blockedMask.value);
        isValid = onBuildable && noOverlap;

        // 5) 색 갱신
        var col = isValid ? validColor : invalidColor;
        SetPreviewColor(col);
        rangeIndicator.SetColor(col);
    }




    /// <summary>
    /// 화면 좌표와 레이어마스크를 받아 실제 표면(World) 포인트를 찾아준다.
    /// 성공 시 true, 실패 시 false.
    /// </summary>
    private bool TryGetSurfacePoint(Ray ray, int surfaceMask, out Vector3 point)
    {
        // ★★★ 애초에 RaycastAll에서 surfaceMask로 필터
        var hits = Physics.RaycastAll(ray, rayMaxDistance, surfaceMask, QueryTriggerInteraction.Collide);
        if (hits != null && hits.Length > 0)
        {
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            point = hits[0].point;       // ★★★ 첫 히트면 충분 (가장 가까운 표면)
            return true;
        }

        // ★★★ 다운캐스트도 동일 마스크 사용
        Vector3 probeStart = ray.origin + ray.direction.normalized * 1000f;
        if (Physics.Raycast(probeStart, Vector3.down, out var downHit, Mathf.Infinity, surfaceMask, QueryTriggerInteraction.Collide))
        {
            point = downHit.point;
            return true;
        }

        point = default;
        return false;
    }

    void EdgePanIfNearScreenBorder()
    {
        if (cameraController == null || previewRoot == null) return;

        // 1) 기준 영역(세이프에어리어 or 전체 화면)
        Rect area = useSafeAreaForEdge ? Screen.safeArea : new Rect(0, 0, Screen.width, Screen.height);

        // 2) 줌 스케일 옵션 vs 퍼센트 방식
        float edgeX, edgeY;

        // 퍼센트 우선: 기기별/해상도별 체감 고정
        edgeX = Mathf.Max(minEdgePx, area.width * edgePctX);
        edgeY = Mathf.Max(minEdgePx, area.height * edgePctY);

        // (원한다면 기존 ‘줌에 따른 px 보간’과 혼용도 가능하지만,
        //  모바일 일관성을 위해 퍼센트를 기본으로 추천)

        // 3) 스크린 좌표(픽셀)
        Vector3 sp = cam.WorldToScreenPoint(previewRoot.transform.position);

        // 세이프에어리어 안 기준선
        float left = area.xMin + edgeX;
        float right = area.xMax - edgeX;
        float bottom = area.yMin + edgeY;
        float top = area.yMax - edgeY;

        float dx = 0f, dy = 0f;

        if (sp.x < left) dx = -Mathf.Pow((left - sp.x) / edgeX, edgePanResponseExponent);
        else if (sp.x > right) dx = Mathf.Pow((sp.x - right) / edgeX, edgePanResponseExponent);

        if (sp.y < bottom) dy = -Mathf.Pow((bottom - sp.y) / edgeY, edgePanResponseExponent);
        else if (sp.y > top) dy = Mathf.Pow((sp.y - top) / edgeY, edgePanResponseExponent);

        if (dx != 0f || dy != 0f)
        {
            float strength = edgePanStrength;
            if (Application.isMobilePlatform)
                strength *= mobileEdgePanStrengthMul;   // 모바일 체감 속도 보정

            cameraController.PanByScreenDir(new Vector2(dx, dy), strength);
        }
    }


    bool TryGetBuildableAnchor(Vector3 probePoint, out Vector3 anchorPos)
    {
        // probePoint 위→아래로 빌더블만 체크
        if (Physics.Raycast(probePoint + Vector3.up * 2f, Vector3.down,
                            out var hit, 6f, buildableMask.value, QueryTriggerInteraction.Collide))
        {
            var t = hit.collider.transform;

            // 1) 자식 Anchor 우선
            Transform anchor = null;
            if (!string.IsNullOrEmpty(anchorChildName))
            {
                anchor = t.Find(anchorChildName);
                if (anchor == null)
                {
                    // 활성/비활성 포함한 자식들 중에서도 탐색
                    var allChildren = t.GetComponentsInChildren<Transform>(true);
                    foreach (var child in allChildren)
                        if (child.name == anchorChildName) { anchor = child; break; }
                }
            }

            // 2) 앵커 없으면 collider 중심
            anchorPos = anchor != null ? anchor.position : hit.collider.bounds.center;

            // 선택 Y 오프셋
            anchorPos += new Vector3(0f, anchorYOffset, 0f);

            // Y는 실제 표면으로 다시 붙이기
            int surfaceMask = followSurfaceMask.value;
            if (Physics.Raycast(anchorPos + Vector3.up * 10f, Vector3.down,
                                out var groundHit, 100f, surfaceMask, QueryTriggerInteraction.Collide))
            {
                anchorPos.y = groundHit.point.y;
            }

            return true;
        }

        anchorPos = probePoint; // 패드 아님: 포인터 지점 그대로
        return false;
    }


    void SetPreviewColor(Color c)
    {
        foreach (var r in previewRenderers)
        {
            r.GetPropertyBlock(mpb);
            if (r.sharedMaterial.HasProperty("_BaseColor")) mpb.SetColor("_BaseColor", c);
            else if (r.sharedMaterial.HasProperty("_Color")) mpb.SetColor("_Color", c);
            r.SetPropertyBlock(mpb);
        }
    }

    void TryPlace()
    {
        if (!isValid) return;
        if (!GameManager.Instance.TrySpendStagePoints(price)) return;

        TowerFactory.Instance.CreateTower(lv, dataSO.levelData, previewRoot.transform.position, null);
        CancelPlacement();
    }

    public void CancelPlacement()
    {
        if (previewRoot != null) Destroy(previewRoot);
        previewRoot = null;
        dataSO = null;
        lv = null;
        isPlacing = false;

        if (cameraController != null) cameraController.SetUserPan(true); // 설치 종료 → 팬 복구
    }
}
