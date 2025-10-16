using UnityEngine;

public class PhysicsDragFinal : MonoBehaviour
{
    private Camera cam;
    private SpringJoint grabJoint;
    private Rigidbody grabbedRb;

    private Rigidbody firstPointRb;
    private Vector3 firstPointAnchorLocal;
    private Vector3 firstGlobalPoint;
    private SpringJoint grabJoint1;
    private SpringJoint grabJoint2;
    private float initialGrabDistance;
    Vector3 jointsOffset = Vector3.zero;


    [Header("잡기 설정")]
    [SerializeField] private float grabMaxDistance = 10f;

    [Header("조인트 설정")]
    [SerializeField] private float springStiffness = 2000f;
    [SerializeField] private float springDamper = 20f;
    [SerializeField] private float jointBreakForce = 500f;

    [Header("마우스 휠 설정")]
    [SerializeField] private float scrollSensitivity = 2f;
    [SerializeField] private float minGrabDistance = 1f;
    [SerializeField] private float maxGrabDistance = 20f;
    private bool doubleGrapping = false;



    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Ray rayForDebug = cam.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(rayForDebug.origin, rayForDebug.direction * grabMaxDistance, Color.green);

        if (Input.GetMouseButtonDown(0))
            TryGrab();
        else if (Input.GetMouseButtonDown(1))
        {
            Release();
            TryDoubleGrab();
        }

        if (Input.GetMouseButtonUp(0))
            Release();

        if (grabJoint != null)
        {
            HandleMouseWheel();
        }
    }

    void FixedUpdate()
    {
        if (grabJoint != null)
            Drag();
        if (grabJoint1 != null)
            DragDoubleGrab();
    }

    void TryDoubleGrab()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, grabMaxDistance))
        {
            if (doubleGrapping)
            {
                ReleaseDoubleGrab();
            }
            else if (firstPointRb == null)
            {
                firstPointRb = hit.collider.attachedRigidbody;
                firstPointAnchorLocal = firstPointRb.transform.InverseTransformPoint(hit.point);
                firstGlobalPoint = hit.point;
                Debug.Log("첫 번째 지점 선택! 같은 물체를 다시 우클릭하여 두 번째 지점을 선택하세요.");
            }
            else if (hit.collider.attachedRigidbody != firstPointRb)
            {
                firstPointRb = hit.collider.attachedRigidbody;
                firstPointAnchorLocal = firstPointRb.transform.InverseTransformPoint(hit.point);
                firstGlobalPoint = hit.point;
                Debug.Log("첫 번째 지점 선택! 같은 물체를 다시 우클릭하여 두 번째 지점을 선택하세요.");
            }
            else
            {
                doubleGrapping = true;
                grabbedRb = firstPointRb;
                initialGrabDistance = hit.distance;

                grabJoint1 = grabbedRb.gameObject.AddComponent<SpringJoint>();
                grabJoint1.autoConfigureConnectedAnchor = false;
                grabJoint1.anchor = firstPointAnchorLocal;
                ConfigureJoint(grabJoint1);

                grabJoint2 = grabbedRb.gameObject.AddComponent<SpringJoint>();
                grabJoint2.autoConfigureConnectedAnchor = false;
                grabJoint2.anchor = grabbedRb.transform.InverseTransformPoint(hit.point);

                ConfigureJoint(grabJoint2);
                jointsOffset = hit.point - firstGlobalPoint;

                Debug.Log(grabbedRb.name + "을(를) 두 지점으로 잡았습니다.");
            }
        }
    }



    void ConfigureJoint(SpringJoint joint)
    {
        joint.spring = springStiffness;
        joint.damper = springDamper;
        joint.breakForce = jointBreakForce;
    }

    void TryGrab()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, grabMaxDistance))
        {
            if (hit.collider.attachedRigidbody != null)
            {
                ReleaseDoubleGrab();
                grabbedRb = hit.collider.attachedRigidbody;
                initialGrabDistance = hit.distance;

                grabJoint = grabbedRb.gameObject.AddComponent<SpringJoint>();
                grabJoint.autoConfigureConnectedAnchor = false;
                grabJoint.anchor = grabbedRb.transform.InverseTransformPoint(hit.point);
                grabJoint.spring = springStiffness;
                grabJoint.damper = springDamper;

                grabJoint.breakForce = jointBreakForce;
            }
        }
    }

    void HandleMouseWheel()
    {
        float scrollInput = Input.mouseScrollDelta.y;
        if (scrollInput != 0)
        {
            initialGrabDistance += scrollInput * scrollSensitivity;
            initialGrabDistance = Mathf.Clamp(initialGrabDistance, minGrabDistance, maxGrabDistance);
        }
    }

    void Drag()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPoint = ray.GetPoint(initialGrabDistance);
        grabJoint.connectedAnchor = targetPoint;
    }

    void DragDoubleGrab()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPoint = ray.GetPoint(initialGrabDistance);
        grabJoint1.connectedAnchor = targetPoint - jointsOffset;
        grabJoint2.connectedAnchor = targetPoint;
    }

    void Release()
    {
        if (grabJoint != null)
        {
            Destroy(grabJoint);
            grabJoint = null;
            grabbedRb = null;
        }
    }


    void ReleaseDoubleGrab()
    {
        if (grabJoint1 != null)
        {
            Destroy(grabJoint1);
            grabJoint1 = null;
        }
        if (grabJoint2 != null)
        {
            Destroy(grabJoint2);
            grabJoint2 = null;
        }
        grabbedRb = null;
        doubleGrapping = false;
    }

    void OnJointBreak(float breakForce)
    {
        Debug.LogWarning("조인트가 끊어졌습니다! 가해진 힘: " + breakForce);
        grabJoint = null;
        grabbedRb = null;
    }
}


