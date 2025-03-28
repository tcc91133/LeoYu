using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthController : MonoBehaviour
{
    public static PlayerHealthController instance;

    private void Awake()
    {
        instance = this;
    }

    public float currentHealth, maxHealth;
    // Start is called before the first frame update

    public Slider healthSlider;

    public GameObject deathEffect;

    // 新增變數
    public SpriteRenderer playerSprite; // 玩家的SpriteRenderer組件
    public float flashDuration = 0.1f; // 變紅持續時間
    private Color originalColor; // 儲存原始顏色

    void Start()
    {
        maxHealth = PlayerStatController.instance.health[0].value;
        currentHealth = maxHealth;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        // 初始化
        if (playerSprite == null)
        {
            playerSprite = GetComponent<SpriteRenderer>();
        }
        if (playerSprite != null)
        {
            originalColor = playerSprite.color;
        }
    }

    public void TakeDamage(float damageToTake)
    {
        SFXManager.instance.PlaySFXPitched(8);
        currentHealth -= damageToTake;

        // 新增：受到傷害時變紅
        if (playerSprite != null)
        {
            StartCoroutine(DamageFlash());
        }

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);

            LevelManager.instance.EndLevel();

            Instantiate(deathEffect, transform.position, transform.rotation);

            SFXManager.instance.PlaySFXPitched(16);

            //SFXManager.instance.PlaySFX(3);

            EnemySpawner[] spawners = FindObjectsOfType<EnemySpawner>();
            foreach (var spawner in spawners)
            {
                spawner.StopSpawning();
            }
        }

        healthSlider.value = currentHealth;
    }

    // 新增：傷害閃爍效果協程
    private IEnumerator DamageFlash()
    {
        // 變紅
        playerSprite.color = Color.red;

        // 等待短暫時間
        yield return new WaitForSeconds(flashDuration);

        // 恢復原始顏色
        playerSprite.color = originalColor;
    }
}