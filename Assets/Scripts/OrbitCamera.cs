using UnityEngine;

/// <summary>
/// 마우스 입력을 사용해 타겟 주위를 공전하고 줌하는 카메라 스크립트입니다.
/// </summary>
public class OrbitCamera : MonoBehaviour
{
    [Header("타겟 설정")]
    [Tooltip("카메라가 바라볼 타겟 오브젝트입니다.")]
    [SerializeField] private Transform target;

    [Header("궤도 및 줌 설정")]
    [Tooltip("타겟으로부터의 초기 거리입니다.")]
    [SerializeField] private float distance = 5.0f;
    [Tooltip("마우스를 사용한 수평/수직 회전 속도입니다.")]
    [SerializeField] private float xSpeed = 120.0f;
    [SerializeField] private float ySpeed = 120.0f;
    [Tooltip("마우스 휠을 사용한 줌 속도입니다.")]
    [SerializeField] private float zoomSpeed = 5.0f;

    [Header("제한 값")]
    [Tooltip("카메라의 최소/최대 고도(수직 각도)입니다.")]
    [SerializeField] private float yMinLimit = -20f;
    [SerializeField] private float yMaxLimit = 80f;
    [Tooltip("카메라의 최소/최대 줌 거리입니다.")]
    [SerializeField] private float distanceMin = .5f;
    [SerializeField] private float distanceMax = 15f;

    // 현재 카메라의 회전 각도를 저장하는 변수
    private float x = 0.0f;
    private float y = 0.0f;

    // 스크립트가 시작될 때 한 번 호출됩니다.
    void Start()
    {
        // 현재 카메라의 오일러 각도를 초기값으로 설정합니다.
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    // 모든 Update 함수가 호출된 후 프레임마다 호출됩니다.
    // 카메라 관련 로직은 LateUpdate에 작성하는 것이 좋습니다.
    void LateUpdate()
    {
        // 타겟이 설정되어 있는지 확인합니다.
        if (target)
        {
            // 마우스 우클릭을 누르고 있는 동안에만 카메라를 조작합니다.
            if (Input.GetMouseButton(1))
            {
                // 마우스의 움직임에 따라 x, y 회전 값을 업데이트합니다.
                // Time.deltaTime을 곱해주면 프레임 속도에 관계없이 일정한 속도를 유지할 수 있습니다.
                x += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
                y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;

                // y(수직) 회전 각도를 지정된 최소/최대 값 사이로 제한합니다.
                y = ClampAngle(y, yMinLimit, yMaxLimit);

                // 마우스 스크롤 휠 입력으로 거리를 조절합니다.
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                distance = Mathf.Clamp(distance - scroll * zoomSpeed, distanceMin, distanceMax);
            }

            // 계산된 회전 값으로 Quaternion을 생성합니다.
            Quaternion rotation = Quaternion.Euler(y, x, 0);

            // 타겟 위치에서 계산된 거리와 회전 값을 적용하여 카메라의 목표 위치를 계산합니다.
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            // 계산된 위치와 회전 값을 카메라의 transform에 적용합니다.
            transform.rotation = rotation;
            transform.position = position;
        }
    }

    /// <summary>
    /// 각도를 주어진 최소값과 최대값 사이로 제한하는 헬퍼 함수입니다.
    /// </summary>
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}