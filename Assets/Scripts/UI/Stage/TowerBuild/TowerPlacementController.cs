using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


/// <summary>
/// 타워 배치 전체 흐름을 관리하는 컨트롤러.
/// - 입력(마우스/터치)을 스크린 좌표로 수집하고
/// - 스크린 레이를 월드 표면으로 투영해 프리뷰를 "어디든" 따라가게 하며(followSurfaceMask)
/// - Buildable 위에선 중앙(또는 Anchor)로 부드럽게 안착(SmoothDamp)
/// - 밖에선 포인터를 부드럽게 추적 + (선택) 그리드 스냅
/// - 설치 가능 판정(Buildable + Blocked 미겹침) 및 프리뷰/사거리 색상 갱신(MaterialPropertyBlock)
/// - 프리뷰가 화면 가장자리 띠에 들어오면 엣지팬 발동(세이프에어리어/퍼센트 기반)
/// </summary>
public class TowerPlacementController : MonoBehaviour
{
    public static TowerPlacementController Instance { get; private set; }

    [Header("카메라(설치 모드 제어)")]
    [SerializeField] private CameraController cameraController;     // CameraController 컴포넌트
    [SerializeField] private float edgePanStrength = 2.0f;           // 엣지팬 가중치(카메라 이동 속도에 곱해짐)

    [Header("Ray Probe")]
    public float rayMaxDistance = 5000f;             // 스크린 레이 최대 길이(멀리 있는 지형도 추적 가능)

    [Header("레이어")]
    public LayerMask groundMask;               // (참고) 바닥 레이어
    public LayerMask buildableMask;            // 설치 가능 패드 레이어 묶음 (비트 마스크)
    public LayerMask blockedMask;              // 길/바위/기존타워 등 설치 금지 충돌체 레이어 묶음

    [Header("표면 추적(프리뷰 이동) 마스크")]
    [SerializeField] private LayerMask followSurfaceMask = ~0; // 타워 프리뷰가 따라갈 표면(나무/바위 위라도 이동만은 가능하게)

    [Header("프리뷰/스냅")]
    public Material previewMat;             // 타워 프리뷰용 머티리얼
    public float footprintRadius = 0.6f;    // 설치 시 겹침 체크 반경
    public bool snapToGrid = true;          // 패드 밖에서만 그리드 스냅 적용
    public float cellSize = 1f;

    [Header("색상")]
    public Color validColor = new(0, 1, 0, 0.45f);      // 타워 배치 가능 영역(녹색, 반투명)
    public Color invalidColor = new(1, 0, 0, 0.45f);    // 타워 배치 불가 영역(적색, 반투명)

    [Header("에디터 키(테스트)")]
    public Key confirmKey = Key.Enter;                  // 설치 확정 테스트 키
    public Key cancelKey = Key.Escape;                  // 설치 취소 테스트 키

    [Header("엣지팬 인식 범위(퍼센트/세이프에어리어)")]
    [SerializeField] private bool useSafeAreaForEdge = true;            // 노치/젯터 영역 제외한 세이프에어리어 기준
    [SerializeField, Range(0f, 0.3f)] private float edgePctX = 0.08f;   // 좌우 가장자리 엣지 인식 띠 두께(화면폭의 %)
    [SerializeField, Range(0f, 0.3f)] private float edgePctY = 0.08f;   // 상하 가장자리 엣지 인식 띠 두께(화면높이의 %)
    [SerializeField] private float minEdgePx = 40f;                     // 작은 폰에서 너무 얇아지지 않도록 최소 픽셀 보장
    [SerializeField] private float mobileEdgePanStrengthMul = 1.8f;     // 유니티 에디터 대비 모바일 체감 속도 보정
    [SerializeField] private float edgePanResponseExponent = 1.2f;      // 엣지 근접도 → 속도 곡선(1=선형, >1=부드럽게)

    [Header("패드 중앙 스냅(부드럽게)")]
    [SerializeField] private float snapSmoothTime = 0.12f;          // 패드 위에서 중앙으로 안착하는 스무딩 시간
    [SerializeField] private float unsnapSmoothTime = 0.06f;        // 패드 밖에서 포인터 추적 스무딩 시간
    [SerializeField] private string anchorChildName = "Anchor";     // 패드에 이 이름의 자식이 있으면 그 위치로 스냅(선택)
    [SerializeField] private float anchorYOffset = 0f;              // 앵커 기준 Y 오프셋(선택)
    

    Vector3 smoothVel; // SmoothDamp용 속도 버퍼(프레임 간 누적 상태)
    // 캐시용
    Camera cam;
    GameObject previewRoot;
    Renderer[] previewRenderers;
    MaterialPropertyBlock mpb;      // 머티리얼 인스턴스 복제 없이 렌더러 값만 변경
    RangeIndicator rangeIndicator;

    // 현재 배치하기 위한 타워 데이터
    TowerDataSO dataSO;
    TowerLevelData lv;
    int price;
    bool isPlacing, isValid;

    public bool IsPlacing => isPlacing; // 외부 읽기 전용 캡슐화
    public static float LastPlacementTime { get; private set; }

    /// <summary>
    /// 싱글턴 초기화 + 카메라/MPB 캐시
    /// </summary>
    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        cam = Camera.main;
        mpb = new MaterialPropertyBlock();
    }


    /// <summary>
    /// 배치 모드 루프.
    /// - 입력 수집(스크린 좌표 + 릴리즈)
    /// - 프리뷰 이동/설치 가능 판정
    /// - 엣지팬 감지/요청
    /// - 설치/취소 키 처리
    /// </summary>
    void Update()
    {
        if (!isPlacing) return;

        if (TryGetPointer(out var screenPos, out bool released, out int pointerId))
        {
            // UI 위에서 조작 중인 경우 설치 확정 중단
            bool overUI = false;
            if (EventSystem.current != null)
            {
                if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
                    overUI = EventSystem.current.IsPointerOverGameObject(pointerId); // 터치
                else
                    overUI = EventSystem.current.IsPointerOverGameObject();          // 마우스
            }

            UpdatePreviewPoseAndValidity(screenPos);    // 타워 프리뷰 이동/설치 가능 판정
            EdgePanIfNearScreenBorder();                // 엣지팬 트리거

            // 설치 확정 (터치, 마우스 업 or 엔터)
            if (!overUI && (released || (Keyboard.current != null && Keyboard.current[confirmKey].wasPressedThisFrame)))
                TryPlace();

            // 설치 취소 (esc)
            if (Keyboard.current != null && Keyboard.current[cancelKey].wasPressedThisFrame)
                CancelPlacement();
        }
    }


    /// <summary>
    /// 외부(UI)에서 호출: 타워 배치 모드 시작(프리뷰 생성/세팅)
    /// </summary>
    public void BeginPlacement(TowerDataSO so, TowerLevelData level)
    {
        CancelPlacement();  // 기존 프리뷰 정리

        dataSO = so;
        lv = level;
        price = level.upgradeCost;

        previewRoot = Instantiate(level.towerPrefab);

        // 타워 프리뷰가 자기 자신 레이캐스트에 걸리지 않게 처리
        previewRoot.layer = LayerMask.NameToLayer("Ignore Raycast");
        foreach (var c in previewRoot.GetComponentsInChildren<Collider>(true))
            c.enabled = false; // 타워 프리뷰 충돌 제거(드래그 방해X)

        // 프리뷰 머티리얼 적용(공유 머티리얼 사용 → MPB로 색만 바꿈)
        previewRenderers = previewRoot.GetComponentsInChildren<Renderer>(true);
        if (previewMat != null)
            foreach (var r in previewRenderers) r.sharedMaterial = previewMat;

        // 사거리 라인렌더러 세팅
        rangeIndicator = previewRoot.GetComponent<RangeIndicator>() ?? previewRoot.AddComponent<RangeIndicator>();
        rangeIndicator.Attach(previewRoot.transform);
        rangeIndicator.SetRadius(level.range);

        // 초기 상태
        isPlacing = true;
        SetPreviewColor(invalidColor);
        rangeIndicator.SetColor(invalidColor);
        smoothVel = Vector3.zero; // 타워 배치가능 영역 초기화

        // 설치 모드 동안 유저 드래그 팬 비활성 (엣지팬만 허용)
        if (cameraController != null) cameraController.SetUserPan(false);
    }


    /// <summary>
    /// 현재 플랫폼에 맞게 입력 포인터(마우스/터치) 통합 획득.
    /// pos: 스크린 좌표(픽셀), released: 이번 프레임에 업 되었는지, pointerId: 터치용 ID
    /// </summary>
    bool TryGetPointer(out Vector2 pos, out bool released, out int pointerId)
    {
        released = false;
        pointerId = -1;

        // 터치 (New Input System)
        var ts = Touchscreen.current;
        if (ts != null)
        {
            var touch = ts.primaryTouch;
            if (touch.press.isPressed || touch.press.wasReleasedThisFrame || touch.press.wasPressedThisFrame)
            {
                pos = touch.position.ReadValue();   // 스크린 픽셀 좌표
                released = touch.press.wasReleasedThisFrame;
                pointerId = touch.touchId.ReadValue();
                return true;
            }
        }

        //마우스
        var mouse = Mouse.current;
        if (mouse != null)
        {
            pos = mouse.position.ReadValue();   // 스크린 픽셀 좌표
            released = mouse.leftButton.wasReleasedThisFrame;
            pointerId = -1;
            return true;
        }

        pos = default;
        return false;
    }


    /// <summary>
    /// 프리뷰 이동/스냅/설치 가능 판정/색 갱신까지 한 번에 수행.
    /// - followSurfaceMask로 ‘이동은 어디든 가능’
    /// - 설치는 buildableMask/blockedMask로 ‘가능/불가’만 나눔
    /// </summary>
    void UpdatePreviewPoseAndValidity(Vector2 screenPos)
    {
        int surfaceMask = followSurfaceMask.value;  // LayerMask -> Int
        var ray = cam.ScreenPointToRay(screenPos);  // 스크린 -> World Ray

        // 1) 프리뷰가 '따라갈' 표면 포인트(어디든 OK)
        if (!TryGetSurfacePoint(ray, surfaceMask, out var followPoint))
        {
            isValid = false;
            return;
        }

        // 2) Buildable 위라면 중앙/앵커로, 아니면 포인터 위치
        Vector3 targetPos;
        bool overBuildable = TryGetBuildableAnchor(followPoint, out targetPos);

        // Buildable 밖일 때만 그리드 스냅 + 높이 보정(다운캐스트)
        if (!overBuildable && snapToGrid)
        {
            targetPos.x = Mathf.Round(targetPos.x / cellSize) * cellSize;
            targetPos.z = Mathf.Round(targetPos.z / cellSize) * cellSize;

            if (Physics.Raycast(targetPos + Vector3.up * 20f, Vector3.down,
                out var snapHit, 100f, surfaceMask, QueryTriggerInteraction.Collide))
            {
                targetPos.y = snapHit.point.y;  // 표면 높이에 딱 붙이기
            }
        }

        // 3) SmoothDamp로 자연스럽게 이동 (Buildable 위/밖 각각 다른 time)
        float smoothTime = overBuildable ? snapSmoothTime : unsnapSmoothTime;
        previewRoot.transform.position =
            Vector3.SmoothDamp(previewRoot.transform.position, targetPos, ref smoothVel, smoothTime);

        // 4) 설치 가능 판정:
        //   - 현재 프리뷰 위치 바로 아래에 Buildable 있는가?
        //   - Blocked 반경 겹침이 없는가?
        Vector3 p = previewRoot.transform.position;
        bool onBuildable = Physics.Raycast(p + Vector3.up * 0.5f, Vector3.down, out _, 2f, buildableMask.value);
        //bool noOverlap = !Physics.CheckSphere(p + Vector3.up * 0.2f, footprintRadius, blockedMask.value);
        var overlaps = Physics.OverlapSphere(p + Vector3.up * 0.2f, footprintRadius, blockedMask.value, QueryTriggerInteraction.Collide);
        bool noOverlap = overlaps == null || overlaps.Length == 0;
        isValid = onBuildable && noOverlap;

        // 5) MPB로 프리뷰 색만 갱신(머티리얼 인스턴스 증가 방지)
        var col = isValid ? validColor : invalidColor;
        SetPreviewColor(col);
        rangeIndicator.SetColor(col);
    }


    /// <summary>
    /// 엣지팬 감지: SafeArea(또는 전체 화면) 안쪽에 ‘띠’를 만들고,
    /// 프리뷰가 띠 안에 있으면 카메라를 해당 방향으로 이동시키도록 CameraController에 요청.
    /// - 퍼센트 기반(해상도/비율 무관 일관성)
    /// - exponent로 가장자리에 가까울수록 속도 곡선 부드럽게
    /// </summary>
    void EdgePanIfNearScreenBorder()
    {
        if (cameraController == null || previewRoot == null) return;

        // 1) 기준 영역 : SafeArea를 쓰면 노치/제스처 구역 제외
        Rect area = useSafeAreaForEdge ? Screen.safeArea : new Rect(0, 0, Screen.width, Screen.height);

        // 2) 퍼센트 기반 띠 두께(px) + 최소 픽셀 보장
        float edgeX = Mathf.Max(minEdgePx, area.width * edgePctX);
        float edgeY = Mathf.Max(minEdgePx, area.height * edgePctY);

        // 3) 프리뷰의 스크린 좌표(픽셀)
        Vector3 sp = cam.WorldToScreenPoint(previewRoot.transform.position);

        // 4) 띠의 내부 경계선(왼/오/아래/위)
        float left = area.xMin + edgeX;
        float right = area.xMax - edgeX;
        float bottom = area.yMin + edgeY;
        float top = area.yMax - edgeY;

        // 5) -1..+1로 정규화된 방향(엣지에 가까울수록 절대값↑)
        float dx = 0f, dy = 0f;

        if (sp.x < left) dx = -Mathf.Pow((left - sp.x) / edgeX, edgePanResponseExponent);
        else if (sp.x > right) dx = Mathf.Pow((sp.x - right) / edgeX, edgePanResponseExponent);

        if (sp.y < bottom) dy = -Mathf.Pow((bottom - sp.y) / edgeY, edgePanResponseExponent);
        else if (sp.y > top) dy = Mathf.Pow((sp.y - top) / edgeY, edgePanResponseExponent);

        // 6) 실제 카메라 이동 요청
        if (dx != 0f || dy != 0f)
        {
            float strength = edgePanStrength;
            if (Application.isMobilePlatform)
                strength *= mobileEdgePanStrengthMul;   // 모바일 체감 속도 보정

            cameraController.PanByScreenDir(new Vector2(dx, dy), strength);
        }
    }


    /// <summary>
    /// 스크린 레이로 표면 포인트 찾기.
    /// 1) RaycastAll(surfaceMask): 가장 가까운 히트 사용(나무/바위 포함)
    /// 2) 실패 시 레이를 멀리 보낸 뒤 위에서 아래로 다운캐스트(대략 지면에 붙임)
    /// </summary>
    private bool TryGetSurfacePoint(Ray ray, int surfaceMask, out Vector3 point)
    {
        // QueryTriggerInteraction.Collide: 트리거도 맞추고 싶다면 Collide(기본은 UseGlobal)
        var hits = Physics.RaycastAll(ray, rayMaxDistance, surfaceMask, QueryTriggerInteraction.Collide);
        if (hits != null && hits.Length > 0)
        {
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));    // 가장 가까운 표면
            point = hits[0].point;       
            return true;
        }

        // 대체 방법 : 전방 멀리 점을 잡고, 위→아래로 찍어 표면 고도 맞춤
        Vector3 probeStart = ray.origin + ray.direction.normalized * 1000f;
        if (Physics.Raycast(probeStart, Vector3.down, out var downHit, Mathf.Infinity, surfaceMask, QueryTriggerInteraction.Collide))
        {
            point = downHit.point;
            return true;
        }

        point = default;
        return false;
    }


    /// <summary>
    /// probePoint 아래로 Buildable만 쏴서, 앵커 자식이 있으면 그 위치로, 없으면 Collider 중심으로 스냅 타깃 반환.
    /// 반환값: true=패드 위(스냅 대상), false=패드 밖(포인터 추적)
    /// </summary>
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

        anchorPos = probePoint; // Buildable 아니면 그대로 포인터 지점
        return false;
    }

    /// <summary>
    /// MaterialPropertyBlock을 사용해 프리뷰 렌더러의 컬러만 변경.
    /// - 머티리얼 인스턴스 복제를 피하고, 드로우콜/메모리 증가 방지.
    /// </summary>
    void SetPreviewColor(Color c)
    {
        foreach (var r in previewRenderers)
        {
            r.GetPropertyBlock(mpb);    // 기존 블록 읽기(덮어쓰기 방지)
            if (r.sharedMaterial.HasProperty("_BaseColor")) mpb.SetColor("_BaseColor", c);
            else if (r.sharedMaterial.HasProperty("_Color")) mpb.SetColor("_Color", c);
            r.SetPropertyBlock(mpb);    // 렌더러에만 적용(머티리얼 공유체는 그대로)
        }
    }

    /// <summary>
    /// 설치 확정: 비용 소모 성공 시 실제 타워 생성 후 배치 모드 종료.
    /// </summary>
    void TryPlace()
    {
        if (!isValid) return;   // 설치 불가 위치
        if (!GameManager.Instance.TrySpendStagePoints(price)) return;   // 비용 부족

        TowerFactory.Instance.CreateTower(dataSO, lv, previewRoot.transform.position, null);
        LastPlacementTime = Time.unscaledTime;  // 타워 설치 직후 선택 방지위한 시간 지정
        CancelPlacement();
    }

    /// <summary>
    /// 프리뷰 정리 + 상태 초기화 + 유저 드래그 팬 복원.
    /// </summary>
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
