using UnityEngine;
using UnityEngine.UI;
using System.Collections; // 添加這行以使用 IEnumerator

public class BossHealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Slider healthSlider;
    public Image damageFlashImage;

    [Header("Settings")]
    public Color flashColor = new Color(1, 0, 0, 0.3f);
    public float flashDuration = 0.15f;
    public float updateInterval = 0.1f;

    private EnemyController bossController;
    private float currentHealth;
    private Coroutine flashRoutine;

    void Start()
    {
        bossController = GetComponentInParent<EnemyController>();

        if (healthSlider != null && bossController != null)
        {
            healthSlider.maxValue = bossController.health;
            healthSlider.value = bossController.health;
            currentHealth = bossController.health;
        }

        if (damageFlashImage != null)
        {
            damageFlashImage.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0);
            damageFlashImage.raycastTarget = false;
        }

        InvokeRepeating(nameof(UpdateHealth), updateInterval, updateInterval);
    }

    void UpdateHealth()
    {
        if (bossController == null || healthSlider == null) return;

        float newHealth = bossController.health;

        if (currentHealth > newHealth)
        {
            if (damageFlashImage != null && flashRoutine == null)
            {
                flashRoutine = StartCoroutine(FlashDamageEffect());
            }
        }

        healthSlider.value = newHealth;
        currentHealth = newHealth;

        if (newHealth <= 0)
        {
            gameObject.SetActive(false);
            CancelInvoke(nameof(UpdateHealth));
        }
    }

    IEnumerator FlashDamageEffect() // 修正為非泛型版本
    {
        if (damageFlashImage == null) yield break;

        // 閃爍效果
        damageFlashImage.color = flashColor;
        yield return new WaitForSeconds(flashDuration);

        // 淡出效果
        float fadeDuration = 0.2f;
        float elapsed = 0;
        Color startColor = flashColor;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        while (elapsed < fadeDuration)
        {
            damageFlashImage.color = Color.Lerp(startColor, endColor, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        damageFlashImage.color = endColor;
        flashRoutine = null;
    }

    void OnDestroy()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
    }
}