using UnityEngine;
using System.Collections;

public class BossLandingEffect : MonoBehaviour
{
    [Header("基本設定")]
    public float damage = 1f;
    public float knockbackForce = 5f;
    public Vector2 knockbackDirection = new Vector2(1f, 0.5f);

    [Header("階段設定")]
    public float secondPhaseMultiplier = 1.5f;  // 第二階段放大倍率
    public float finalPhaseMultiplier = 2f;     // 最終階段放大倍率
    public float phaseDuration = 0.3f;

    [Header("視覺效果")]
    public SpriteRenderer effectSprite;
    public float flashDuration = 0.1f;
    private Color originalColor;
    private Vector3 originalScale;  // 儲存Prefab原始Scale

    private void Start()
    {
        // 記錄Prefab原始Scale
        originalScale = transform.localScale;

        if (effectSprite != null)
        {
            originalColor = effectSprite.color;
        }

        StartCoroutine(ThreePhaseEffect());
    }

    private IEnumerator ThreePhaseEffect()
    {
        // 第一階段：原始大小
        transform.localScale = originalScale;
        yield return new WaitForSeconds(phaseDuration);

        // 第二階段：等比例放大
        transform.localScale = originalScale * secondPhaseMultiplier;
        yield return new WaitForSeconds(phaseDuration);

        // 第三階段：再次等比例放大
        transform.localScale = originalScale * finalPhaseMultiplier;
        yield return new WaitForSeconds(phaseDuration);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealthController.instance.TakeDamage(damage);
            KnockbackPlayer(other.transform);

            if (effectSprite != null)
            {
                StartCoroutine(EffectFlash());
            }
        }
    }

    private void KnockbackPlayer(Transform player)
    {
        Vector2 dir = (player.position - transform.position).normalized;
        Vector2 finalKnockback = new Vector2(
            knockbackDirection.x * dir.x,
            knockbackDirection.y
        ).normalized * knockbackForce;

        if (player.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.velocity = finalKnockback;
        }
    }

    private IEnumerator EffectFlash()
    {
        effectSprite.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        effectSprite.color = originalColor;
    }
}