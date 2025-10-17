using UnityEngine;
using System;

// Drags objects and provides information about the grabbed object to other systems.
public class ObjectDragger : MonoBehaviour
{
    // Public property to expose the exact 3D point where the object was grabbed.
    // Nullable Vector3 (?) allows us to easily check if an object is currently being held.
    public Vector3? WorldGrabPoint { get; private set; }

    private Rigidbody selectedRigidbody;
    private Camera mainCamera;
    private float selectionDistance;
    private bool isCameraRotating = false;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        CameraController.OnCameraRotationStart += HandleCameraRotationStart;
        CameraController.OnCameraRotationEnd += HandleCameraRotationEnd;
    }

    private void OnDisable()
    {
        CameraController.OnCameraRotationStart -= HandleCameraRotationStart;
        CameraController.OnCameraRotationEnd -= HandleCameraRotationEnd;
    }

    void Update()
    {
        if (isCameraRotating) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.rigidbody != null)
            {
                selectedRigidbody = hit.rigidbody;
                selectionDistance = hit.distance;

                // Store the precise 3D grab point. This is the key information.
                // 정확한 3D 잡기 지점을 저장합니다. 이것이 핵심 정보입니다.
                WorldGrabPoint = hit.point;

                selectedRigidbody.isKinematic = true;
                Debug.Log($"ObjectDragger: Grabbed {selectedRigidbody.name} at {WorldGrabPoint.Value}");
            }
        }

        if (Input.GetMouseButtonUp(0) && selectedRigidbody != null)
        {
            selectedRigidbody.isKinematic = false;
            selectedRigidbody = null;

            // Clear the grab point when the object is released.
            // 오브젝트를 놓으면 잡기 지점을 비웁니다.
            WorldGrabPoint = null;
            Debug.Log("ObjectDragger: Released object.");
        }
    }

    void FixedUpdate()
    {
        if (selectedRigidbody != null && !isCameraRotating)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Vector3 targetPosition = ray.GetPoint(selectionDistance);
            selectedRigidbody.MovePosition(targetPosition);
        }
    }

    private void HandleCameraRotationStart() => isCameraRotating = true;
    private void HandleCameraRotationEnd() => isCameraRotating = false;
}