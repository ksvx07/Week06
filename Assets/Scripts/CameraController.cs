using UnityEngine;
using System;

// Orchestrates camera rotation and tells other systems (like CursorManager) how to behave.
public class CameraController : MonoBehaviour
{
    public static event Action OnCameraRotationStart;
    public static event Action OnCameraRotationEnd;

    [SerializeField] private Transform target;
    [SerializeField] private float rotationSpeed = 3.0f;

    // References to the other managers.
    private CursorManager cursorManager;
    private ObjectDragger objectDragger;

    void Start()
    {
        // Find the managers in the scene.
        cursorManager = FindObjectOfType<CursorManager>();
        objectDragger = FindObjectOfType<ObjectDragger>();

        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // Ask the ObjectDragger if it's currently holding something.
            // ObjectDragger에게 현재 무언가 잡고 있는지 물어봅니다.
            Vector3? grabPoint = objectDragger.WorldGrabPoint;

            if (grabPoint.HasValue)
            {
                // If YES, tell the cursor to track that 3D point.
                // 잡고 있다면, 커서에게 그 3D 지점을 추적하라고 명령합니다.
                cursorManager.StartTrackingWorldPoint(grabPoint.Value);
            }
            else
            {
                // If NO, just hide the cursor as before.
                // 잡고 있지 않다면, 이전처럼 커서를 숨깁니다.
                cursorManager.HideCursor();
            }

            Cursor.lockState = CursorLockMode.Locked;
            OnCameraRotationStart?.Invoke();
        }

        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
            transform.RotateAround(target.position, Vector3.up, mouseX);
            transform.RotateAround(target.position, transform.right, -mouseY);
        }

        if (Input.GetMouseButtonUp(1))
        {
            // No matter what, stop tracking and show the cursor to return to normal state.
            // 어떤 상태였든, 추적을 멈추고 커서를 보여줘서 기본 상태로 되돌립니다.
            cursorManager.StopTracking();
            cursorManager.ShowCursor();

            Cursor.lockState = CursorLockMode.Confined;
            OnCameraRotationEnd?.Invoke();
        }
    }
}