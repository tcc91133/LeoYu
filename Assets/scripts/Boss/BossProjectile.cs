using UnityEngine;
using System.Collections;

public class BossProjectile : MonoBehaviour
{
    [Header("基本設定")]
    public float damage = 1f;
    public float duration = 2f;
    public float speed = 5f;

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

        // 移除原本的方向計算，因為現在完全由父物件的旋轉控制
        StartCoroutine(AutoDestroy());
    }

    private void Update()
    {
        // 簡化移動邏輯，直接朝物件的右方(本地座標)移動
        // 因為物件已經被旋轉到正確方向
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