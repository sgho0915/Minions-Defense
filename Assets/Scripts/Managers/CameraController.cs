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
/// # 요약
/// - 드래그 팬 속도와 엣지팬 속도를 분리해 개별 제어
/// - 엣지팬 속도는 인스펙터/런타임에서 변경 가능 (SetEdgePanSpeed)
/// - 필요 시 줌 비율에 따라 엣지팬 속도 스케일 옵션 제공
/// </summary>
public class CameraController : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Gizmo(Debug)")]
    [SerializeField] private bool drawBoundsGizmo = true;
    [SerializeField] private Color boundsColor = new(0f, 0.8f, 1f, 0.8f);
    [SerializeField] private float gizmoY = 0f;
    [SerializeField] private bool useTargetY = true;
    [SerializeField] private Collider stageCollider;
#endif

    [Header("Camera Target & Component")]
    [SerializeField] private Transform cameraTarget;         // CinemachineCamera.Follow
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [Header("Pan Settings")]
    [SerializeField] private float dragPanSpeed = 8f;        // ★★★ 드래그 팬 속도(기존 panSpeed 분리)
    [SerializeField] private float edgePanSpeed = 8f;        // ★★★ 엣지팬 전용 속도(월드유닛/초)
    [SerializeField] private bool scaleEdgePanWithZoom = true;   // ★★★ 줌에 따라 엣지팬 속도 스케일링
    [SerializeField] private Vector2 edgePanZoomScaleRange = new(0.75f, 1.5f); // ★★★ (min,max)

    [Header("Zoom Settings")]
    [SerializeField] private float wheelZoomSpeed = 0.05f;
    [SerializeField] private float pinchZoomSpeed = 0.05f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 20f;

    [Header("Stage Bounds (XZ)")]
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    public bool UserPanEnabled { get; private set; } = true;
    /// <summary>설치 모드 등에서 사용자 드래그 팬 허용/차단</summary>
    public void SetUserPan(bool enabled) => UserPanEnabled = enabled;

    /// <summary>엣지팬 속도를 런타임에 변경</summary> // ★★★ 추가
    public void SetEdgePanSpeed(float unitsPerSecond) => edgePanSpeed = Mathf.Max(0f, unitsPerSecond);  // ★★★

    private CinemachinePositionComposer composer;
    private float prevPinchDist;

    private void OnEnable()
    {
#if UNITY_IOS || UNITY_ANDROID
        EnhancedTouchSupport.Enable();
#endif
    }

    private void Awake()
    {
        composer = cinemachineCamera != null
            ? cinemachineCamera.GetComponent<CinemachinePositionComposer>()
            : null;

        if (composer == null)
            Debug.LogError("CinemachinePositionComposer가 CinemachineCamera에 설정되어 있어야 합니다.");
    }

    private void Update()
    {
        HandleUserPan();
        HandleWheelZoom();

#if UNITY_IOS || UNITY_ANDROID
        HandlePinchZoom();
        if (Touch.activeTouches.Count < 2) prevPinchDist = 0f;
#endif
    }

    /// <summary>
    /// 현재 줌 비율 헬퍼
    /// </summary>
    public float GetZoom01()
    {
        if (composer == null || Mathf.Approximately(maxZoom, minZoom)) return 0f;
        return Mathf.InverseLerp(minZoom, maxZoom, composer.CameraDistance); // 0=최소줌, 1=최대줌
    }

    /// <summary>마우스/터치 드래그 팬</summary>
    private void HandleUserPan()
    {
        if (!UserPanEnabled) return;

        var cam = Camera.main;
        if (cam == null) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.isPressed) return;

        Vector2 delta = Mouse.current.delta.ReadValue();
        if (delta.sqrMagnitude < 1e-6f) return;

        Vector3 right = cam.transform.right; right.y = 0; right.Normalize();
        Vector3 fwd = cam.transform.forward; fwd.y = 0; fwd.Normalize();

        Vector3 move = (-delta.x * right + -delta.y * fwd) * dragPanSpeed * Time.deltaTime; // ★★★ dragPanSpeed 사용
        cameraTarget.position += move;
        ClampTargetPosition();

#elif UNITY_IOS || UNITY_ANDROID
        if (Touch.activeTouches.Count == 1)
        {
            var t = Touch.activeTouches[0];
            if (t.phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                Vector2 delta = t.delta;
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

    /// <summary>마우스 휠 줌</summary>
    private void HandleWheelZoom()
    {
        if (composer == null) return;
        if (Mouse.current == null) return;

        float scrollY = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Abs(scrollY) < 0.01f) return;

        float newDist = composer.CameraDistance - scrollY * wheelZoomSpeed;
        composer.CameraDistance = Mathf.Clamp(newDist, minZoom, maxZoom);
    }

    /// <summary>모바일 핀치 줌</summary>
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
        if (prevPinchDist == 0f) { prevPinchDist = currentDist; return; }

        float delta = currentDist - prevPinchDist;
        prevPinchDist = currentDist;

        float newDist = composer.CameraDistance - delta * pinchZoomSpeed * Time.deltaTime;
        composer.CameraDistance = Mathf.Clamp(newDist, minZoom, maxZoom);
#endif
    }

    /// <summary>
    /// 엣지팬: 화면 경계 근처일 때 카메라를 스크린 방향으로 이동
    /// dir01: -1..+1 범위(좌우/상하), strength: 호출 측(예: 설치 컨트롤러) 가중치
    /// </summary>
    public void PanByScreenDir(Vector2 dir01, float strength = 1f)
    {
        if (dir01.sqrMagnitude < 1e-6f) return;
        var cam = Camera.main; if (cam == null) return;

        Vector3 right = cam.transform.right; right.y = 0; right.Normalize();
        Vector3 fwd = cam.transform.forward; fwd.y = 0; fwd.Normalize();

        // ★★★ 엣지팬 전용 속도 사용 + (선택) 줌 스케일 적용
        float speed = edgePanSpeed; // ★★★
        if (scaleEdgePanWithZoom && composer != null) // ★★★
        {
            float t = Mathf.InverseLerp(minZoom, maxZoom, composer.CameraDistance);
            speed *= Mathf.Lerp(edgePanZoomScaleRange.x, edgePanZoomScaleRange.y, t);
        }

        Vector3 delta = (right * dir01.x + fwd * dir01.y) * speed * strength * Time.deltaTime; // ★★★
        cameraTarget.position += delta;
        ClampTargetPosition();
    }

    /// <summary>카메라 타겟 위치를 XZ 경계로 클램프</summary>
    private void ClampTargetPosition()
    {
        Vector3 pos = cameraTarget.position;
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.z = Mathf.Clamp(pos.z, minBounds.y, maxBounds.y);
        cameraTarget.position = pos;
    }

    private void OnDisable()
    {
#if UNITY_IOS || UNITY_ANDROID
        EnhancedTouchSupport.Disable();
#endif
    }

#if UNITY_EDITOR
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
