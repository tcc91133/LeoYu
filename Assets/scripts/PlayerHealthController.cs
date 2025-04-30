using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealthController : MonoBehaviour
{
    public static PlayerHealthController instance;

    [Header("Health Settings")]
    public float currentHealth, maxHealth;
    public Slider healthSlider;
    public GameObject deathEffect;

    [Header("Damage Effect")]
    public SpriteRenderer playerSprite;
    public float flashDuration = 0.1f;

    private Color originalColor;
    private Coroutine damageFlashCoroutine;
    private bool isDead;

    private void Awake() => instance = this;

    void Start()
    {
        maxHealth = PlayerStatController.instance.health[0].value;
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        if (playerSprite == null) playerSprite = GetComponent<SpriteRenderer>();
        originalColor = playerSprite != null ? playerSprite.color : Color.white;
    }

    public void TakeDamage(float damageToTake)
    {
        if (isDead || !gameObject.activeInHierarchy) return;

        SFXManager.instance.PlaySFXPitched(8);
        currentHealth = Mathf.Max(0, currentHealth - damageToTake);
        healthSlider.value = currentHealth;

        if (damageFlashCoroutine != null) StopCoroutine(damageFlashCoroutine);
        damageFlashCoroutine = StartCoroutine(DamageFlash());

        if (currentHealth <= 0) Die();
    }

    private IEnumerator DamageFlash()
    {
        if (playerSprite != null)
        {
            playerSprite.color = Color.red;
            yield return new WaitForSeconds(flashDuration);
            playerSprite.color = originalColor;
        }
        damageFlashCoroutine = null;
    }

    private void Die()
    {
        isDead = true;
        StopAllCoroutines();

        GetComponent<PlayerDash>()?.OnDeath();
        GetComponent<PlayerController>().enabled = false;

        Instantiate(deathEffect, transform.position, Quaternion.identity);
        SFXManager.instance.PlaySFXPitched(16);

        foreach (var spawner in FindObjectsOfType<EnemySpawner>())
            spawner.StopSpawning();

        StartCoroutine(DeactivateAfterFrame());
    }

    private IEnumerator DeactivateAfterFrame()
    {
        yield return null;
        gameObject.SetActive(false);
        LevelManager.instance.EndLevel();
    }
}