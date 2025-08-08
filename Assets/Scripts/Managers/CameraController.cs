
// CameraController.cs
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using UnityEditor;

#if UNITY_IOS || UNITY_ANDROID
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
#endif


/// <summary>
/// Cinemachine 카메라의 Follow 타겟을 이동/줌하며, 경계 제한과 모바일 핀치줌까지 처리하는 컨트롤러
/// </summary>
public class CameraController : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Gizmo(Debug)")]                                           
    [SerializeField] private bool drawBoundsGizmo = true;               // 경계 사각형 표시 on/off
    [SerializeField] private Color boundsColor = new(0f, 0.8f, 1f, 0.8f); // 선 색상/투명도
    [SerializeField] private float gizmoY = 0f;                         // 사각형을 그릴 높이(Y)
    [SerializeField] private bool useTargetY = true;                    // 카메라 타겟의 Y를 사용할지
    [Tooltip("지정 시 [Fit Bounds From Collider] 메뉴로 콜라이더의 XZ 크기를 min/max로 자동 설정합니다.")]
    [SerializeField] private Collider stageCollider;                    // 스테이지 콜라이더(선택)
#endif

    [Header("Camera Target & Component")]
    [SerializeField] private Transform cameraTarget; // Cinemachine이 따라갈 대상
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [Header("Pan Settings")]
    [SerializeField] private float panSpeed = 0.05f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float pinchZoomSpeed = 0.05f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 20f;

    [Header("Stage Bounds")]
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;

    [SerializeField] private InputActionAsset inputActions;


    private InputAction panAction;
    private InputAction zoomAction;

    private float prevPinchDist;

    private CinemachinePositionComposer composer; 

    private void OnEnable()
    {
#if UNITY_IOS || UNITY_ANDROID
        EnhancedTouchSupport.Enable();
#endif
    }

    private void Awake()
    {
        if (inputActions == null)
        {
            Debug.LogError("InputActionAsset이 할당되지 않았습니다.");
            return;
        }



        panAction = inputActions.FindActionMap("Camera").FindAction("Pan");
        zoomAction = inputActions.FindActionMap("Camera").FindAction("Zoom");

        panAction.Enable();
        zoomAction.Enable();

        zoomAction.performed += ctx =>
        {
            float scroll = ctx.ReadValue<float>();

            if (composer != null)
            {
                float newDist = composer.CameraDistance - scroll * zoomSpeed;
                composer.CameraDistance = Mathf.Clamp(newDist, minZoom, maxZoom);
            }
        };


        composer = cinemachineCamera.GetComponent<CinemachinePositionComposer>();
        if (composer == null)
        {
            Debug.LogError("CinemachinePositionComposer가 CinemachineCamera에 설정되어 있어야 합니다.");
        }
    }

    private void Update()
    {
        HandlePan();
#if UNITY_IOS || UNITY_ANDROID
        HandlePinchZoom();

        if (Touch.activeTouches.Count < 2)
        {
            prevPinchDist = 0f;
        }
#endif
    }

    /// <summary>
    /// 마우스 또는 터치 입력으로 카메라 타겟 이동
    /// </summary>
    private void HandlePan()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        // 마우스 클릭된 경우에만 팬
        if (!Mouse.current.leftButton.isPressed)
            return;
#elif UNITY_IOS || UNITY_ANDROID                           
        if (Touch.activeTouches.Count >= 2)
            return; // ★★★ 두 손가락 이상이면 팬 비활성화
#endif

        Vector2 panInput = panAction.ReadValue<Vector2>();
        if (panInput == Vector2.zero)
            return;

        Vector3 move = new Vector3(-panInput.x, 0f, -panInput.y) * panSpeed;
        cameraTarget.position += move;

        ClampTargetPosition();
    }

    /// <summary>
    /// 마우스 휠 입력에 따른 줌 처리
    /// </summary>
    private void HandleZoom()
    {
        float scroll = zoomAction.ReadValue<float>();
        Debug.Log($"Scroll: {scroll}");

        if (Mathf.Approximately(scroll, 0f))
            return;

        if (composer != null)
        {
            float newDist = composer.CameraDistance - scroll * zoomSpeed * Time.deltaTime;
            composer.CameraDistance = Mathf.Clamp(newDist, minZoom, maxZoom);
        }
    }

    /// <summary>
    /// 모바일용 핀치 줌 처리
    /// </summary>
    private void HandlePinchZoom()
    {
#if UNITY_IOS || UNITY_ANDROID
        if (Touch.activeTouches.Count < 2)
            return;

        var t0 = Touch.activeTouches[0];
        var t1 = Touch.activeTouches[1];

        // 두 손가락 중 하나라도 움직이지 않으면 무시
        if (t0.phase != UnityEngine.InputSystem.TouchPhase.Moved &&
            t1.phase != UnityEngine.InputSystem.TouchPhase.Moved)
            return;

        Vector2 pos0 = t0.screenPosition;
        Vector2 pos1 = t1.screenPosition;

        float currentDist = Vector2.Distance(pos0, pos1);

        if (prevPinchDist == 0f)
        {
            prevPinchDist = currentDist;
            return;
        }

        float delta = currentDist - prevPinchDist;
        prevPinchDist = currentDist;

        if (Mathf.Abs(delta) > 0.01f && composer != null)
        {
            float newDist = composer.CameraDistance - delta * pinchZoomSpeed * Time.deltaTime;
            composer.CameraDistance = Mathf.Clamp(newDist, minZoom, maxZoom);
        }
#endif
    }

    /// <summary>
    /// 화면 이탈 방지를 위한 위치 제한
    /// </summary>
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
        panAction.Disable();
        zoomAction.Disable();
    }

#if UNITY_EDITOR
    /// <summary>
    /// 씬 뷰에서 카메라 이동 한계(min/max)를 직사각형으로 표시
    /// </summary>
    private void OnDrawGizmosSelected()                              
    {
        if (!drawBoundsGizmo) return;

        // 그릴 높이 결정
        float y = useTargetY && cameraTarget != null ? cameraTarget.position.y : gizmoY;

        float minX = minBounds.x;
        float maxX = maxBounds.x;
        float minZ = minBounds.y;
        float maxZ = maxBounds.y;

        Vector3 c = new((minX + maxX) * 0.5f, y, (minZ + maxZ) * 0.5f);
        Vector3 size = new(Mathf.Abs(maxX - minX), 0.01f, Mathf.Abs(maxZ - minZ));

        Gizmos.color = boundsColor;
        Gizmos.DrawWireCube(c, size);                                  

        // 코너점/축 라인 약간 보강(가시성)
        Vector3 a = new(minX, y, minZ);
        Vector3 b = new(maxX, y, minZ);
        Vector3 d = new(minX, y, maxZ);
        Vector3 e = new(maxX, y, maxZ);
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(b, e);
        Gizmos.DrawLine(e, d);
        Gizmos.DrawLine(d, a);

#if UNITY_EDITOR
        // 라벨로 범위 표시
        Handles.Label(c + Vector3.up * 0.1f,
            $"Bounds X:[{minX:F2} ~ {maxX:F2}]  Z:[{minZ:F2} ~ {maxZ:F2}]");
#endif
    }

    /// <summary>
    /// 지정한 스테이지 콜라이더의 월드 바운드를 읽어 min/max를 자동 세팅 (수동 실행)
    /// </summary>
    [ContextMenu("Fit Bounds From Collider")]                          
    private void FitBoundsFromCollider()
    {
        if (stageCollider == null)
        {
            Debug.LogWarning("Stage Collider가 비어있습니다.");
            return;
        }

        Bounds b = stageCollider.bounds;
        // XZ 평면 기준으로 세팅
        minBounds = new Vector2(b.min.x, b.min.z);                     
        maxBounds = new Vector2(b.max.x, b.max.z);                     
        // 기즈모 Y는 콜라이더 중앙 높이로 맞춰주면 보기 좋음
        gizmoY = b.center.y;
        Debug.Log($"[Bounds Fitted] X:[{minBounds.x:F2}~{maxBounds.x:F2}] Z:[{minBounds.y:F2}~{maxBounds.y:F2}]");
    }
#endif
}
