using UnityEngine;

// Manages the visual custom cursor, including a manual positioning mode.
public class CursorManager : SingletonObject<CursorManager>
{
    [SerializeField] private RectTransform cursorUITransform;
    [SerializeField] private float manualMoveSpeed = 15f;
    private bool isCursorActive = false;
    public Vector3 CursorPosition;
    private bool isTrackingWorldPoint = false;
    private Vector3 trackedWorldPoint;
    protected override void Awake()
    {
        base.Awake();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!cursorUITransform.gameObject.activeInHierarchy) return;

        if (!isCursorActive)
        {
            // Manual Control Mode: Move the UI cursor based on mouse delta.
            Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * manualMoveSpeed;

            cursorUITransform.position += new Vector3(delta.x, delta.y, 0); ;
            CursorPosition = cursorUITransform.position;
            // Keep the cursor within the screen boundaries.
            ClampCursorToScreen();
        }
        else
        {

        }
    }

    // --- Public Methods for other scripts to call ---

    public void ShowCursor()
    {
        cursorUITransform.gameObject.SetActive(true);
    }

    public void HideCursor()
    {
        cursorUITransform.gameObject.SetActive(false);
    }

    // Enables manual cursor movement.
    public void EnableManualControl()
    {
        isCursorActive = true;
    }

    // Disables manual control, reverting to default behavior.
    public void DisableManualControl()
    {
        isCursorActive = false;
    }

    // --- Helper Method ---

    // Prevents the cursor from going off-screen.
    private void ClampCursorToScreen()
    {
        Vector3 pos = cursorUITransform.position;
        pos.x = Mathf.Clamp(pos.x, 0, Screen.width);
        pos.y = Mathf.Clamp(pos.y, 0, Screen.height);
        cursorUITransform.position = pos;
    }

    public void StartTrackingWorldPoint(Vector3 worldPoint)
    {
        trackedWorldPoint = worldPoint;
        isTrackingWorldPoint = true;
        ShowCursor(); // Ensure cursor is visible while tracking.
        Debug.Log($"CursorManager: Now tracking world point {worldPoint}");
    }

    // Returns the cursor to its default mouse-following behavior.
    public void StopTracking()
    {
        isTrackingWorldPoint = false;
        Debug.Log("CursorManager: Stopped tracking, reverting to default.");
    }
}

