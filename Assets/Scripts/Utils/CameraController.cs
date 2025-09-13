using UnityEngine;
using Unity.Cinemachine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_IOS || UNITY_ANDROID
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
#endif

using UnityEngine.InputSystem;

/// <summary>
/// 카메라 제어(팔로우 타겟 기반):
/// - 드래그 팬(사용자가 화면을 끌어 타겟 이동) ON/OFF
/// - 엣지팬(외부에서 dir01 전달)으로 타겟 이동
/// - 휠/핀치 줌: CinemachinePositionComposer.CameraDistance 조절
/// - XZ 경계로 타겟만 클램프(Confiner3D는 카메라 ‘자체’를 클램프)
///   → 둘이 보완 관계(타겟 범위 + 카메라 충돌/가둠)
/// </summary>
public class CameraController : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Gizmo(Debug)")]
    [SerializeField] private bool drawBoundsGizmo = true;
    [SerializeField] private Color boundsColor = new(0f, 0.8f, 1f, 0.8f);
    [SerializeField] private float gizmoY = 0f;
    [SerializeField] private bool useTargetY = true;
    [SerializeField] private Collider stageCollider;    // [ContextMenu]로 min/maxBounds를 자동 세팅할 때 사용
#endif

    [Header("Camera Target & Component")]
    [SerializeField] private Transform cameraTarget;         // CinemachineCamera.TrackingTarget 대상
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [Header("Pan Settings")]
    [SerializeField] private float dragPanSpeed = 8f;               // 유저 드래그 팬 속도 (월드 유닛/초)
    [SerializeField] private float edgePanSpeed = 8f;               // 엣지팬 전용 속도(월드유닛/초)
    [SerializeField] private bool scaleEdgePanWithZoom = true;      // 줌 비율에 따라 엣지팬 속도 스케일링
    [SerializeField] private Vector2 edgePanZoomScaleRange = new(0.75f, 1.5f); // 최소값 ~ 최대값 배율

    [Header("Zoom Settings")]
    [SerializeField] private float wheelZoomSpeed = 0.05f;    // 마우스 휠 감도
    [SerializeField] private float pinchZoomSpeed = 0.05f;    // 핀치 감도
    [SerializeField] private float minZoom = 5f;              // composer.CameraDistance 최소값
    [SerializeField] private float maxZoom = 20f;             // composer.CameraDistance 최대값

    [Header("Stage Bounds (XZ)")]
    [SerializeField] private Vector2 minBounds;               // Follow 타켓 XZ 경계(왼쪽/아래)
    [SerializeField] private Vector2 maxBounds;               // Follow 타켓 XZ 경계(오른쪽/위)

    public bool UserPanEnabled { get; private set; } = true;    // 유저 드래그 팬 허용 여부 (설치모드에서 off)
    public void SetUserPan(bool enabled) => UserPanEnabled = enabled;

    public void SetEdgePanSpeed(float unitsPerSecond) => edgePanSpeed = Mathf.Max(0f, unitsPerSecond);  // 런타임 엣지팬 속도 튜닝

    private CinemachinePositionComposer composer;   // 줌, 프레이밍 담당
    private float prevPinchDist;                    // 핀치 이전 길이(프레임 간 저장)


    /// <summary>모바일 터치 고급 입력 활성화</summary>
    private void OnEnable()
    {
#if UNITY_IOS || UNITY_ANDROID
        EnhancedTouchSupport.Enable();
#endif
    }


    /// <summary>
    /// 필수 컴포넌트 캐싱(Composer) 및 유효성 체크.
    /// CinemachineCamera에 CinemachinePositionComposer가 있어야 CameraDistance 제어 가능.
    /// </summary>
    private void Awake()
    {
        composer = cinemachineCamera != null
            ? cinemachineCamera.GetComponent<CinemachinePositionComposer>()
            : null;

        if (composer == null)
            Debug.LogError("CinemachinePositionComposer가 CinemachineCamera에 설정되어 있어야 합니다.");
    }


    /// <summary>
    /// 매 프레임: 유저 드래그 팬, 마우스 휠 줌, (모바일) 핀치 줌 처리
    /// </summary>
    private void Update()
    {
        HandleUserPan();
        HandleWheelZoom();

#if UNITY_IOS || UNITY_ANDROID
        HandlePinchZoom();
        if (Touch.activeTouches.Count < 2) prevPinchDist = 0f;  // 핀치 종료 시 초기화
#endif
    }


    /// <summary>
    /// 현재 줌 비율(0..1) 반환. minZoom→0, maxZoom→1.
    /// 엣지팬 속도 스케일링 등에 사용.
    /// </summary>
    public float GetZoom01()
    {
        if (composer == null || Mathf.Approximately(maxZoom, minZoom)) return 0f;
        return Mathf.InverseLerp(minZoom, maxZoom, composer.CameraDistance); // 0=최소줌, 1=최대줌
    }


    /// <summary>
    /// 유저 드래그 팬(수평 이동). 설치 모드에서는 UserPanEnabled=false로 차단.
    /// 화면 픽셀 델타 → 카메라의 수평/전방 벡터에 투영 → Follow 타겟 이동.
    /// </summary>
    private void HandleUserPan()
    {
        if (!UserPanEnabled) return;

        var cam = Camera.main;
        if (cam == null) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.isPressed) return;

        Vector2 delta = Mouse.current.delta.ReadValue();    // 화면 픽셀 델타
        if (delta.sqrMagnitude < 1e-6f) return;

        // 카메라 기준 수평/전방을 구해 수평면(XZ)에만 이동
        Vector3 right = cam.transform.right; right.y = 0; right.Normalize();
        Vector3 fwd = cam.transform.forward; fwd.y = 0; fwd.Normalize();

        // 드래그의 반대 방향으로 움직이는 UX(손으로 ‘끌어당기는’ 느낌)
        Vector3 move = (-delta.x * right + -delta.y * fwd) * dragPanSpeed * Time.deltaTime;
        cameraTarget.position += move;
        ClampTargetPosition();  // 타겟 XZ 경계로 제한

#elif UNITY_IOS || UNITY_ANDROID
        if (Touch.activeTouches.Count == 1)
        {
            var t = Touch.activeTouches[0];
            if (t.phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                Vector2 delta = t.delta;        // 화면 픽셀 델타
                var camMain = Camera.main;
                Vector3 right = camMain.transform.right; right.y = 0; right.Normalize();
                Vector3 fwd   = camMain.transform.forward;fwd.y   = 0; fwd.Normalize();

                Vector3 move = (-delta.x * right + -delta.y * fwd) * dragPanSpeed * Time.deltaTime; // ★★★
                cameraTarget.position += move;
                ClampTargetPosition();
            }
        }
#endif
    }


    /// <summary>
    /// 마우스 휠 줌. composer.CameraDistance를 직접 변경.
    /// </summary>
    private void HandleWheelZoom()
    {
        if (composer == null) return;
        if (Mouse.current == null) return;

        float scrollY = Mouse.current.scroll.ReadValue().y;     // 보통 ±120 단위
        if (Mathf.Abs(scrollY) < 0.01f) return;

        float newDist = composer.CameraDistance - scrollY * wheelZoomSpeed;
        composer.CameraDistance = Mathf.Clamp(newDist, minZoom, maxZoom);
    }


    /// <summary>
    /// 모바일 핀치 줌. 두 터치 사이 거리 변화량으로 composer.CameraDistance를 가감.
    /// </summary>
    private void HandlePinchZoom()
    {
#if UNITY_IOS || UNITY_ANDROID
        if (composer == null) return;
        if (Touch.activeTouches.Count < 2) return;

        var t0 = Touch.activeTouches[0];
        var t1 = Touch.activeTouches[1];
        if (t0.phase != UnityEngine.InputSystem.TouchPhase.Moved &&
            t1.phase != UnityEngine.InputSystem.TouchPhase.Moved)
            return;

        float currentDist = Vector2.Distance(t0.screenPosition, t1.screenPosition);
        if (prevPinchDist == 0f) { prevPinchDist = currentDist; return; }   // 첫 프레임 초기화

        float delta = currentDist - prevPinchDist;  // 핀치 거리 변화량
        prevPinchDist = currentDist;

        float newDist = composer.CameraDistance - delta * pinchZoomSpeed * Time.deltaTime;
        composer.CameraDistance = Mathf.Clamp(newDist, minZoom, maxZoom);
#endif
    }


    /// <summary>
    /// 엣지팬 입력(dir01: -1..+1)을 받아 Follow 타겟을 이동.
    /// TowerPlacementController가 EdgePanIfNearScreenBorder에서 방향/세기를 계산해 호출.
    /// </summary>
    public void PanByScreenDir(Vector2 dir01, float strength = 1f)
    {
        if (dir01.sqrMagnitude < 1e-6f) return;
        var cam = Camera.main; if (cam == null) return;

        Vector3 right = cam.transform.right; right.y = 0; right.Normalize();
        Vector3 fwd = cam.transform.forward; fwd.y = 0; fwd.Normalize();

        // 엣지팬 전용 속도 사용 + (선택) 줌 스케일 적용
        float speed = edgePanSpeed;
        if (scaleEdgePanWithZoom && composer != null) 
        {
            float t = Mathf.InverseLerp(minZoom, maxZoom, composer.CameraDistance);
            speed *= Mathf.Lerp(edgePanZoomScaleRange.x, edgePanZoomScaleRange.y, t);
        }

        Vector3 delta = (right * dir01.x + fwd * dir01.y) * speed * strength * Time.deltaTime;
        cameraTarget.position += delta;
        ClampTargetPosition();  // 타겟 이동 후 항상 경계 클램프
    }


    /// <summary>
    /// Follow 타겟을 XZ 경계로 제한(게임 플레이 공간을 벗어나지 않도록).
    /// Confiner3D는 카메라 자체를 박스/볼륨으로 제한 → 둘은 보완 관계.
    /// </summary>
    private void ClampTargetPosition()
    {
        Vector3 pos = cameraTarget.position;
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.z = Mathf.Clamp(pos.z, minBounds.y, maxBounds.y);
        cameraTarget.position = pos;
    }


    /// <summary>모바일 고급 터치 입력 비활성화</summary>
    private void OnDisable()
    {
#if UNITY_IOS || UNITY_ANDROID
        EnhancedTouchSupport.Disable();
#endif
    }


#if UNITY_EDITOR
    /// <summary>
    /// 에디터에서 Bounds 시각화. (타겟 클램프 영역)
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!drawBoundsGizmo) return;
        float y = useTargetY && cameraTarget != null ? cameraTarget.position.y : gizmoY;

        float minX = minBounds.x, maxX = maxBounds.x, minZ = minBounds.y, maxZ = maxBounds.y;
        Vector3 c = new((minX + maxX) * 0.5f, y, (minZ + maxZ) * 0.5f);
        Vector3 size = new(Mathf.Abs(maxX - minX), 0.01f, Mathf.Abs(maxZ - minZ));

        Gizmos.color = boundsColor;
        Gizmos.DrawWireCube(c, size);
        Handles.Label(c + Vector3.up * 0.1f, $"Bounds X:[{minX:F2}~{maxX:F2}]  Z:[{minZ:F2}~{maxZ:F2}]");
    }


    /// <summary>
    /// Stage 콜라이더 박스에서 타겟 XZ 경계를 추출(에디터 편의 기능).
    /// </summary>
    [ContextMenu("Fit Bounds From Collider")]
    private void FitBoundsFromCollider()
    {
        if (stageCollider == null) { Debug.LogWarning("Stage Collider가 비어있습니다."); return; }
        Bounds b = stageCollider.bounds;
        minBounds = new Vector2(b.min.x, b.min.z);
        maxBounds = new Vector2(b.max.x, b.max.z);
        gizmoY = b.center.y;
        Debug.Log($"[Bounds Fitted] X:[{minBounds.x:F2}~{maxBounds.x:F2}] Z:[{minBounds.y:F2}~{maxBounds.y:F2}]");
    }
#endif
}
