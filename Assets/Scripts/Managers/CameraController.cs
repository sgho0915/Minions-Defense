
// CameraController.cs
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
#if UNITY_IOS || UNITY_ANDROID
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
#endif


/// <summary>
/// Cinemachine 카메라의 Follow 타겟을 이동/줌하며, 경계 제한과 모바일 핀치줌까지 처리하는 컨트롤러
/// </summary>
public class CameraController : MonoBehaviour
{
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

    private CinemachinePositionComposer composer; // ★★★ 수정됨

    private void OnEnable()
    {
#if UNITY_IOS || UNITY_ANDROID
        EnhancedTouchSupport.Enable(); // ★★★ Awake() 말고 OnEnable에서
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
            Debug.Log($"Scroll performed: {scroll}"); // ★★★ 로그는 이벤트에서 찍자

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
        //HandleZoom();
#if UNITY_IOS || UNITY_ANDROID
        HandlePinchZoom();

        // ★★★ 손가락이 2개 미만일 때 prevPinchDist 초기화
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
}
