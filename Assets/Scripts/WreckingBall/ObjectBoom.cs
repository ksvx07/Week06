using System.Collections;
using UnityEngine;

// 라바콘에 부착하는 스크립트
public class ObjectBoom : MonoBehaviour
{
    [Header("날아가는 효과")]
    [SerializeField] private float knockbackForce = 25f;  // 충돌시 날아가는 힘
    [SerializeField] private float upwardForce = 12f;  // 위로 뜨는 힘
    [SerializeField] private float spinForce = 800f;  // 회전 힘

    [Header("물리 설정")]
    [SerializeField] private float mass = 1f;  // 라바콘 질량

    [Header("디버그")]
    [SerializeField] private bool showDebugInfo = true;

    private Rigidbody rb;
    private Renderer coneRenderer;
    private Collider coneCollider;
    private bool hasBeenHit = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    // 라바콘 충돌 이벤트
    public static System.Action<bool, int> OnConeHit;

    private void Start()
    {
        // Rigidbody 설정
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.mass = mass;
        //rb.isKinematic = true;  // 초기엔 키네마틱 (충돌 전까지 고정)

        // 컴포넌트 가져오기
        coneRenderer = GetComponent<Renderer>();
        coneCollider = GetComponent<Collider>();

        // Collider가 Trigger가 아닌지 확인
        if (coneCollider != null && coneCollider.isTrigger)
        {
            coneCollider.isTrigger = false;
            Debug.LogWarning($"TrafficCone {gameObject.name}: Collider를 Trigger에서 일반 충돌로 변경했습니다.");
        }

        // 원래 위치 저장 (리셋용)
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // 태그 설정 - 중요!
        gameObject.tag = "TrafficCone";
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag != "WreckingBall")
            return;

        ApplyKnockback(collision.contacts[0].point);
    }

    private void ApplyKnockback(Vector3 hitPoint)
    {
        // 물리 활성화
        rb.isKinematic = false;

        // 라바콘이 날아갈 방향 계산 (충돌 지점의 반대 방향)
        Vector3 knockbackDirection = (transform.position - hitPoint).normalized;
        knockbackDirection.y = 0; // 수평 방향만 고려

        float speedMultiplier = 2f;

        // 강력한 힘 적용
        Vector3 force = knockbackDirection * knockbackForce * speedMultiplier;
        force.y = upwardForce * Mathf.Max(1f, speedMultiplier * 0.7f);  // 속도가 빠를수록 더 높이

        // 충격 적용
        rb.AddForce(force, ForceMode.Impulse);

        // 더 역동적인 회전 추가
        Vector3 randomTorque = new Vector3(
            Random.Range(-spinForce, spinForce) * speedMultiplier,
            Random.Range(-spinForce, spinForce) * speedMultiplier,
            Random.Range(-spinForce, spinForce) * speedMultiplier
        );
        rb.AddTorque(randomTorque);

        // 약간의 드래그 감소로 더 멀리 날아가게
        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.5f;
    }


    // 외부에서 충돌 여부 확인용
    public bool HasBeenHit()
    {
        return hasBeenHit;
    }

    // 디버그용 기즈모
    private void OnDrawGizmos()
    {
        if (!hasBeenHit)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
        }
    }
}