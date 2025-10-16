using UnityEngine;

public class PhysicsDragFinal : MonoBehaviour
{
    private LineRenderer line1;
    private LineRenderer line2;
    [Header("라인 렌더러 설정")]
    [SerializeField] private Material lineMaterial; // 라인에 사용할 머티리얼
    [SerializeField] private Gradient stressGradient;
    [SerializeField] private float lineWidth = 0.05f; // 라인 두께
    [SerializeField] private float dangerThreshold = 0.8f; // 위험 색상으로 바뀌는 임계값
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
            }
            else if (hit.collider.attachedRigidbody != firstPointRb)
            {
                firstPointRb = hit.collider.attachedRigidbody;
                firstPointAnchorLocal = firstPointRb.transform.InverseTransformPoint(hit.point);
                firstGlobalPoint = hit.point;
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

                CreateLineRenderer(ref line1);
                CreateLineRenderer(ref line2);

                // 첫 프레임 원점 점프 방지: 즉시 connectedAnchor와 라인 좌표 초기화
                Vector3 targetPoint = ray.GetPoint(initialGrabDistance);
                grabJoint1.connectedAnchor = targetPoint - jointsOffset;
                grabJoint2.connectedAnchor = targetPoint;

                if (line1 != null)
                {
                    line1.SetPosition(0, grabJoint1.transform.TransformPoint(grabJoint1.anchor));
                    line1.SetPosition(1, grabJoint1.connectedAnchor);
                }
                if (line2 != null)
                {
                    line2.SetPosition(0, grabJoint2.transform.TransformPoint(grabJoint2.anchor));
                    line2.SetPosition(1, grabJoint2.connectedAnchor);
                }
            }
        }
    }

    void LateUpdate()
    {
        if (line1 != null)
        {
            // 단일 잡기일 경우 grabJoint, 두 지점 잡기일 경우 grabJoint1 사용
            SpringJoint activeJoint = (grabJoint != null) ? grabJoint : grabJoint1;
            UpdateLine(line1, activeJoint);
        }
        if (line2 != null)
        {
            UpdateLine(line2, grabJoint2);
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
                CreateLineRenderer(ref line1);

                // 첫 프레임 원점 점프 방지: 즉시 connectedAnchor와 라인 좌표 초기화
                Vector3 targetPoint = ray.GetPoint(initialGrabDistance);
                grabJoint.connectedAnchor = targetPoint;
                if (line1 != null)
                {
                    line1.SetPosition(0, grabJoint.transform.TransformPoint(grabJoint.anchor));
                    line1.SetPosition(1, targetPoint);
                }
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
            if (line1 != null) Destroy(line1.gameObject);
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
        if (line1 != null) Destroy(line1.gameObject);
        if (line2 != null) Destroy(line2.gameObject);
    }

    void OnJointBreak(float breakForce)
    {
        Debug.LogWarning("조인트가 끊어졌습니다! 가해진 힘: " + breakForce);
        // grabJoint = null;
        // grabbedRb = null;
        Release();
        ReleaseDoubleGrab();
    }

    void CreateLineRenderer(ref LineRenderer line)
    {
        if (line != null) Destroy(line.gameObject);

        GameObject lineObj = new GameObject("GrabLine");
        line = lineObj.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.material = lineMaterial;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.numCapVertices = 10;
        line.useWorldSpace = true; // 월드 좌표 사용을 명시해 위치 초기화가 즉시 반영되도록
    }

    void UpdateLine(LineRenderer line, SpringJoint joint)
    {
        // 조인트가 파괴되면 line은 있지만 joint는 null일 수 있음
        if (joint == null || joint.connectedBody != null)
        {
            if (line != null) Destroy(line.gameObject);
            return;
        }

        // 라인의 시작점 (오브젝트의 앵커)과 끝점 (조인트의 목표) 설정
        line.SetPosition(0, joint.transform.TransformPoint(joint.anchor));
        line.SetPosition(1, joint.connectedAnchor);

        float stress = Mathf.Clamp01((joint.currentForce.magnitude / joint.breakForce) / dangerThreshold);

        Color stressColor = stressGradient.Evaluate(stress);
        line.startColor = stressColor;
        line.endColor = stressColor;
    }
}


