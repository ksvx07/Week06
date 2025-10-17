using Unity.Cinemachine; // 시네머신 네임스페이스 추가
using System.Collections;
using UnityEngine;

/// <summary>
/// Time.timeScale 에러를 방지하는 안전장치가 추가된 최종 버전입니다.
/// </summary>
public class ClimaxController_Advanced : MonoBehaviour
{
    [Header("Target Containers")]
    [Tooltip("파괴할 모든 블록을 포함하는 최상위 부모 오브젝트입니다.")]
    [SerializeField] private Transform jengaBlocksContainer;
    [Tooltip("파괴할 모든 바닥 블록을 포함하는 최상위 부모 오브젝트입니다.")]
    [SerializeField] private Transform floorBlocksContainer;

    [Header("Bomb Drop")]
    [Tooltip("투하할 폭탄 프리팹입니다. 'BombController' 스크립트가 있어야 합니다.")]
    [SerializeField] private GameObject bombPrefab;
    [Tooltip("폭탄이 처음 생성될 위치(Transform)입니다.")]
    [SerializeField] private Transform bombSpawnPoint;
    [Tooltip("폭탄 투하 후, 실제 충격파 폭발이 일어나기까지의 대기 시간입니다.")]
    [SerializeField] private float delayBeforeExplosion = 3.0f;

    [Header("Explosion Settings")]
    [Tooltip("폭발 충격파가 시작될 중심점(Transform)입니다.")]
    [SerializeField] private Transform explosionCenter;
    [Tooltip("폭발의 기본 힘입니다.")]
    [SerializeField] private float explosionForce = 500f;
    [Tooltip("폭발 충격파가 미치는 반경입니다.")]
    [SerializeField] private float explosionRadius = 15f;
    [Tooltip("폭발 시 오브젝트를 위로 띄워 올리는 힘을 추가합니다.")]
    [SerializeField] private float upwardModifier = 2.0f;

    [Header("Advanced FX Settings")]
    [Tooltip("카메라 셰이크를 위한 Cinemachine Impulse Source입니다.")]
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [Tooltip("히트스탑이 시작되기 전까지 대기할 프레임 수입니다.")]
    [SerializeField] private int hitstopDelayFrames = 50;
    [Tooltip("히트스탑(시간 정지) 지속 시간(초)입니다.")]
    [SerializeField] private float hitstopDuration = 0.2f;
    [Tooltip("히트스탑 종료 후, 원래 시간 속도로 돌아오는 데 걸리는 시간(초)입니다.")]
    [SerializeField] private float timeScaleRecoveryDuration = 1.0f;
    [Tooltip("시간 속도 복구 애니메이션의 형태를 정의하는 커브입니다.")]
    [SerializeField] private AnimationCurve timeScaleRecoveryCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);


    [Header("Rigidbody Settings for Floor")]
    [SerializeField] private RigidbodySettings floorRigidbodySettings;

    [System.Serializable]
    public struct RigidbodySettings
    {
        public float mass;
        public float linearDamping;
        public float angularDamping;
        public RigidbodyInterpolation interpolation;
        public CollisionDetectionMode collisionDetectionMode;
    }

    public void StartClimaxSequence()
    {
        StopAllCoroutines();
        StartCoroutine(ClimaxCoroutine());
    }

    private IEnumerator ClimaxCoroutine()
    {
        if (bombPrefab != null && bombSpawnPoint != null)
        {
            GameObject bombInstance = Instantiate(bombPrefab, bombSpawnPoint.position, bombSpawnPoint.rotation);
            BombController bombController = bombInstance.GetComponent<BombController>();
            if (bombController != null)
            {
                bombController.StartTicking(delayBeforeExplosion);
            }
        }

        yield return new WaitForSeconds(delayBeforeExplosion);

        TriggerExplosion();
        StartCoroutine(HitStopCoroutine());
    }

    private void TriggerExplosion()
    {
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }

        if (jengaBlocksContainer != null) ApplyExplosionRecursively(jengaBlocksContainer);
        if (floorBlocksContainer != null) AddRigidbodyAndApplyExplosionRecursively(floorBlocksContainer);
    }

    private IEnumerator HitStopCoroutine()
    {
        for (int i = 0; i < hitstopDelayFrames; i++)
        {
            yield return null;
        }

        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        float waitTimer = 0f;
        while (waitTimer < hitstopDuration)
        {
            waitTimer += Time.unscaledDeltaTime;
            yield return null;
        }

        float elapsedTime = 0f;
        while (elapsedTime < timeScaleRecoveryDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            // [수정됨] curveSamplePoint가 1을 넘지 않도록 Clamp01 처리
            float curveSamplePoint = Mathf.Clamp01(elapsedTime / timeScaleRecoveryDuration);
            float curveValue = timeScaleRecoveryCurve.Evaluate(curveSamplePoint);

            // [수정됨] LerpUnclamped 대신 Lerp를 사용하여 결과값이 0과 originalTimeScale 사이를 벗어나지 않도록 보장
            Time.timeScale = Mathf.Lerp(0f, originalTimeScale, curveValue);

            yield return null;
        }

        Time.timeScale = originalTimeScale;
    }

    private void ApplyExplosionRecursively(Transform parent)
    {
        Rigidbody rb = parent.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddExplosionForce(explosionForce, explosionCenter.position, explosionRadius, upwardModifier);
        }
        foreach (Transform child in parent) ApplyExplosionRecursively(child);
    }

    private void AddRigidbodyAndApplyExplosionRecursively(Transform parent)
    {
        Rigidbody rb = parent.GetComponent<Rigidbody>();
        if (rb == null && parent.GetComponent<Collider>() != null)
        {
            rb = parent.gameObject.AddComponent<Rigidbody>();
            rb.mass = floorRigidbodySettings.mass;
            rb.linearDamping = floorRigidbodySettings.linearDamping;
            rb.angularDamping = floorRigidbodySettings.angularDamping;
            rb.interpolation = floorRigidbodySettings.interpolation;
            rb.collisionDetectionMode = floorRigidbodySettings.collisionDetectionMode;
        }
        if (rb != null)
        {
            rb.AddExplosionForce(explosionForce, explosionCenter.position, explosionRadius, upwardModifier);
        }
        foreach (Transform child in parent) AddRigidbodyAndApplyExplosionRecursively(child);
    }
}

