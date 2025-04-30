using UnityEngine;
using System.Collections;

public class BossLandingEffect : MonoBehaviour
{
    [Header("�򥻳]�w")]
    public float damage = 1f;
    public float knockbackForce = 5f;
    public Vector2 knockbackDirection = new Vector2(1f, 0.5f);

    [Header("���q�]�w")]
    public float secondPhaseMultiplier = 1.5f;  // �ĤG���q��j���v
    public float finalPhaseMultiplier = 2f;     // �̲׶��q��j���v
    public float phaseDuration = 0.3f;

    [Header("��ı�ĪG")]
    public SpriteRenderer effectSprite;
    public float flashDuration = 0.1f;
    private Color originalColor;
    private Vector3 originalScale;  // �x�sPrefab��lScale

    private void Start()
    {
        // �O��Prefab��lScale
        originalScale = transform.localScale;

        if (effectSprite != null)
        {
            originalColor = effectSprite.color;
        }

        StartCoroutine(ThreePhaseEffect());
    }

    private IEnumerator ThreePhaseEffect()
    {
        // �Ĥ@���q�G��l�j�p
        transform.localScale = originalScale;
        yield return new WaitForSeconds(phaseDuration);

        // �ĤG���q�G����ҩ�j
        transform.localScale = originalScale * secondPhaseMultiplier;
        yield return new WaitForSeconds(phaseDuration);

        // �ĤT���q�G�A������ҩ�j
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