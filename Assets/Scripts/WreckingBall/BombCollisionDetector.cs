using UnityEngine;
using UnityEngine.Events;
using System;

/// <summary>
/// 폭탄의 충돌을 감지하고 폭발 이벤트를 발생시킵니다.
/// UnityEvent를 사용하여 Inspector에서 연결 가능합니다.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class BombCollisionDetector : MonoBehaviour
{
    [Header("Collision Detection")]
    [Tooltip("폭발을 트리거할 오브젝트의 태그입니다.")]
    [SerializeField] private string limitLineTag = "LimitLine";
    
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

    private bool hasExploded = false;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        // Rigidbody 검증
        if (rb == null)
        {
            Debug.LogError($"[BombCollisionDetector] {gameObject.name}에 Rigidbody가 없습니다!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 이미 폭발했으면 무시
        if (hasExploded) return;

        // 태그 체크
        if (collision.gameObject.CompareTag(limitLineTag))
        {
            hasExploded = true;
            HandleCollision(collision);
        }
    }

    private void HandleCollision(Collision collision)
    {
        Debug.Log($"[BombCollisionDetector] {gameObject.name}이(가) {collision.gameObject.name}에 충돌! 폭발 요청 전송.");

        // VFX 생성 (충돌 지점)
        if (explosionVFX != null && collision.contacts.Length > 0)
        {
            Vector3 contactPoint = collision.contacts[0].point;
            GameObject vfxInstance = Instantiate(explosionVFX, contactPoint, Quaternion.identity);
            
            // 자동 소멸
            if (vfxLifetime > 0)
            {
                Destroy(vfxInstance, vfxLifetime);
            }
            
            Debug.Log($"[BombCollisionDetector] VFX 생성: {contactPoint}");
        }

        // UnityEvent 호출 (Inspector 연결용)
        onBombCollision?.Invoke(gameObject);

        // C# Event 호출 (코드 구독용)
        OnBombCollisionDetected?.Invoke(gameObject);
    }

    /// <summary>
    /// 외부에서 강제로 폭발 상태를 리셋합니다.
    /// </summary>
    public void ResetExplosionState()
    {
        hasExploded = false;
    }

    /// <summary>
    /// 현재 폭발 상태를 확인합니다.
    /// </summary>
    public bool HasExploded => hasExploded;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 태그 존재 여부 확인
        if (!IsTagValid(limitLineTag))
        {
            Debug.LogWarning($"[BombCollisionDetector] '{limitLineTag}' 태그가 Tag Manager에 등록되어 있지 않습니다.");
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
        // 충돌 범위 시각화 (Collider 기준)
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = hasExploded ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, col.bounds.extents.magnitude);
        }
    }
#endif
}