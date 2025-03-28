using System.Collections;  // 添加這行以使用 IEnumerator
using UnityEngine;
using Pathfinding;

public class EnemyController : MonoBehaviour
{
    // 新增變數
    public SpriteRenderer enemySprite; // 敵人的SpriteRenderer組件
    public float flashDuration = 0.1f; // 變紅持續時間
    private Color originalColor; // 儲存原始顏色

    public Rigidbody2D theRB;
    public float moveSpeed;
    private Transform target;

    public float damage;
    public float hitWaitTime = 1f;
    private float hitCounter;

    public float health = 5f;
    public float knockBackTime = 0.5f;
    private float knockBackCounter;

    public int expToGive = 1;
    public int coinValue = 1;
    public float coinDropRate = .5f;

    private AIPath aiPath;

    void Start()
    {
        theRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        aiPath = GetComponent<AIPath>();

        if (enemySprite == null)
        {
            enemySprite = GetComponent<SpriteRenderer>();
        }
        if (enemySprite != null)
        {
            originalColor = enemySprite.color;
        }

        if (aiPath != null)
        {
            aiPath.constrainInsideGraph = false;
        }
    }

    void Update()
    {
        if (PlayerController.instance != null && PlayerController.instance.gameObject.activeSelf)
        {
            if (knockBackCounter > 0)
            {
                knockBackCounter -= Time.deltaTime;

                if (moveSpeed > 0)
                {
                    moveSpeed = -moveSpeed * 2f;
                }

                if (knockBackCounter <= 0)
                {
                    moveSpeed = Mathf.Abs(moveSpeed * 0.5f);
                }
            }

            if (target != null)
            {
                theRB.velocity = (target.position - transform.position).normalized * moveSpeed;
            }
            else
            {
                theRB.velocity = Vector2.zero;
            }

            if (hitCounter > 0f)
            {
                hitCounter -= Time.deltaTime;
            }
        }
        else
        {
            theRB.velocity = Vector2.zero;

            if (aiPath != null)
            {
                aiPath.enabled = false;
            }

            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && hitCounter <= 0f)
        {
            PlayerHealthController.instance.TakeDamage(damage);
            hitCounter = hitWaitTime;
        }
    }

    public void TakeDamage(float damageToTake)
    {
        health -= damageToTake;
        SFXManager.instance.PlaySFXPitched(13);

        if (enemySprite != null)
        {
            StartCoroutine(DamageFlash());
        }

        if (health <= 0)
        {
            SFXManager.instance.PlaySFXPitched(6);
            Destroy(gameObject);

            ExperienceLevelController.instance.SpawnExp(transform.position, expToGive);

            if (Random.value <= coinDropRate)
            {
                CoinController.instance.DropCoin(transform.position, coinValue);
            }
        }
        DamageNumberController.instance.SpawnDamage(damageToTake, transform.position);
    }

    public void TakeDamage(float damageToTake, bool shouldKnockback)
    {
        TakeDamage(damageToTake);
        if (shouldKnockback)
        {
            knockBackCounter = knockBackTime;
        }
    }

    private IEnumerator DamageFlash()
    {
        enemySprite.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        enemySprite.color = originalColor;
    }
}