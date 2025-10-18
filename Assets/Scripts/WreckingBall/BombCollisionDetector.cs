using UnityEngine;
using UnityEngine.Events;
using System;

/// <summary>
/// LimitLine에 부착되어 폭탄의 충돌을 감지하고 폭발 이벤트를 발생시킵니다.
/// OnTriggerEnter를 사용하여 Bomb 태그를 가진 오브젝트를 감지합니다.
/// </summary>
[RequireComponent(typeof(Collider))]
public class BombCollisionDetector : MonoBehaviour
{
    [Header("Collision Detection")]
    [Tooltip("폭발을 트리거할 오브젝트의 태그입니다.")]
    [SerializeField] private string bombTag = "Bomb";
    
    [Header("Events")]
    [Tooltip("충돌 시 발생하는 UnityEvent입니다. Inspector에서 연결할 수 있습니다.")]
    [SerializeField] private UnityEvent<GameObject> onBombCollision;
    
    [Header("Visual Effects")]
    [Tooltip("충돌 지점에 생성할 폭발 이펙트 프리팹입니다.")]
    [SerializeField] private GameObject explosionVFX;
    [Tooltip("VFX가 자동으로 소멸되는 시간(초)입니다. 0이면 자동 소멸 안 함.")]
    [SerializeField] private float vfxLifetime = 3.0f;

    // C# Event (코드에서 구독용)
    public static event Action<GameObject> OnBombCollisionDetected;

    private Collider triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        
        // Collider 검증 및 Trigger 설정 확인
        if (triggerCollider == null)
        {
            Debug.LogError($"[BombCollisionDetector] {gameObject.name}에 Collider가 없습니다!");
        }
        else if (!triggerCollider.isTrigger)
        {
            Debug.LogWarning($"[BombCollisionDetector] {gameObject.name}의 Collider가 Trigger로 설정되어 있지 않습니다. 자동으로 Trigger를 활성화합니다.");
            triggerCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Bomb 태그 체크
        if (other.CompareTag(bombTag))
        {
            GameObject bomb = other.gameObject;
            HandleTrigger(bomb, other.ClosestPoint(transform.position));
        }
    }

    private void HandleTrigger(GameObject bomb, Vector3 contactPoint)
    {
        Debug.Log($"[BombCollisionDetector] {gameObject.name}이(가) {bomb.name}을(를) 감지! 폭발 요청 전송.");

        // VFX 생성 (접촉 지점)
        if (explosionVFX != null)
        {
            GameObject vfxInstance = Instantiate(explosionVFX, contactPoint, Quaternion.identity);
            
            // 자동 소멸
            if (vfxLifetime > 0)
            {
                Destroy(vfxInstance, vfxLifetime);
            }
            
            Debug.Log($"[BombCollisionDetector] VFX 생성: {contactPoint}");
        }

        // UnityEvent 호출 (Inspector 연결용)
        onBombCollision?.Invoke(bomb);

        // C# Event 호출 (코드 구독용)
        OnBombCollisionDetected?.Invoke(bomb);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 태그 존재 여부 확인
        if (!IsTagValid(bombTag))
        {
            Debug.LogWarning($"[BombCollisionDetector] '{bombTag}' 태그가 Tag Manager에 등록되어 있지 않습니다.");
        }

        // Collider가 Trigger인지 확인
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"[BombCollisionDetector] Collider의 'Is Trigger'를 활성화해야 합니다!");
        }
    }

    private bool IsTagValid(string tag)
    {
        try
        {
            GameObject.FindGameObjectWithTag(tag);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 트리거 범위 시각화 (Collider 기준)
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
            
            // Box Collider
            if (col is BoxCollider boxCol)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(boxCol.center, boxCol.size);
            }
            // Sphere Collider
            else if (col is SphereCollider sphereCol)
            {
                Gizmos.DrawWireSphere(transform.position + sphereCol.center, sphereCol.radius * transform.lossyScale.x);
            }
            // Capsule Collider
            else if (col is CapsuleCollider capsuleCol)
            {
                Gizmos.DrawWireSphere(transform.position + capsuleCol.center, capsuleCol.radius * transform.lossyScale.x);
            }
            // 기타 Collider
            else
            {
                Gizmos.DrawWireSphere(transform.position, col.bounds.extents.magnitude);
            }
        }
    }
#endif
}