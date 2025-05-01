using UnityEngine;
using System.Collections;

public class BossSwordHit : MonoBehaviour
{
    [Header("基本設定")]
    public float damage = 1f;
    public float duration = 0.3f;

    [Header("視覺效果")]
    public SpriteRenderer effectSprite;
    public float flashDuration = 0.1f;
    private Color originalColor;

    private void Start()
    {
        if (effectSprite != null)
        {
            originalColor = effectSprite.color;
        }

        StartCoroutine(AutoDestroy());
    }

    private IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealthController.instance.TakeDamage(damage);

            if (effectSprite != null)
            {
                StartCoroutine(EffectFlash());
            }
        }
    }

    private IEnumerator EffectFlash()
    {
        effectSprite.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        effectSprite.color = originalColor;
    }
}
