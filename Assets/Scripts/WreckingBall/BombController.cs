using UnityEngine;
using System.Collections;

/// <summary>
/// 폭탄의 시각적 효과(점등)와 소멸을 담당하는 스크립트입니다.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class BombController : MonoBehaviour
{
    [Header("Blinking Effect")]
    [Tooltip("점등 시 사용할 빨간색 머티리얼입니다.")]
    [SerializeField] private Material blinkingMaterial;
    [Tooltip("점등 주기(초)입니다. 1초에 한 번씩 깜빡입니다.")]
    [SerializeField] private float blinkInterval = 1.0f;

    private Renderer objectRenderer;
    private Material originalMaterial;

    private void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        originalMaterial = objectRenderer.material; // 원래 머티리얼 저장
    }

    /// <summary>
    /// ClimaxController에 의해 호출되어 점등 및 자동 소멸 시퀀스를 시작합니다.
    /// </summary>
    /// <param name="duration">폭발까지 걸리는 시간(초)</param>
    public void StartTicking(float duration)
    {
        StartCoroutine(TickingCoroutine(duration));
    }

    private IEnumerator TickingCoroutine(float duration)
    {
        float elapsedTime = 0f;

        // 지정된 시간 동안 점등 효과를 반복합니다.
        while (elapsedTime < duration)
        {
            // 빨간색으로 변경
            objectRenderer.material = blinkingMaterial;
            yield return new WaitForSeconds(blinkInterval / 2);

            // 원래 색으로 복귀
            objectRenderer.material = originalMaterial;
            yield return new WaitForSeconds(blinkInterval / 2);

            elapsedTime += blinkInterval;
        }

        // 폭발 직후, 자신을 씬에서 사라지게 합니다.
        gameObject.SetActive(false);
    }
}

