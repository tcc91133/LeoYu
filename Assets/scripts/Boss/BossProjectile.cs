using UnityEngine;
using System.Collections;

public class BossProjectile : MonoBehaviour
{
    [Header("�򥻳]�w")]
    public float damage = 1f;
    public float duration = 2f;
    public float speed = 5f;

    [Header("��ı�ĪG")]
    public SpriteRenderer effectSprite;
    public float flashDuration = 0.1f;
    private Color originalColor;

    private void Start()
    {
        if (effectSprite != null)
        {
            originalColor = effectSprite.color;
        }

        // �����쥻����V�p��A�]���{�b�����Ѥ����󪺱��౱��
        StartCoroutine(AutoDestroy());
    }

    private void Update()
    {
        // ²�Ʋ����޿�A�����ª��󪺥k��(���a�y��)����
        // �]������w�g�Q����쥿�T��V
        transform.Translate(Vector2.right * speed * Time.deltaTime, Space.Self);
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

            Destroy(gameObject);
        }
    }

    private IEnumerator EffectFlash()
    {
        effectSprite.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        effectSprite.color = originalColor;
    }
}