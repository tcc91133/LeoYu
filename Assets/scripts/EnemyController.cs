using UnityEngine;
using Pathfinding;  // 引入 Pathfinding 命名空间以访问 AIPath 组件

public class EnemyController : MonoBehaviour
{

    //private bool hosSpawned;

    public Rigidbody2D theRB;
    public float moveSpeed;
    private Transform target;

    public float damage;

    public float hitWaitTime = 1f;
    private float hitCounter;

    public float health = 5f;

    public float knockBackTime = 0.5f; //击退
    private float knockBackCounter;

    public int expToGive = 1;

    public int coinValue = 1;
    public float coinDropRate = .5f;

    private AIPath aiPath;  // 添加 AIPath 变量，表示 AIPath 组件的实例

    // Start is called before the first frame update
    void Start()
    {
        theRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        aiPath = GetComponent<AIPath>();

        // 禁用 AIPath 的物理碰撞檢測
        if (aiPath != null)
        {
            aiPath.constrainInsideGraph = false; // 禁止強制限制在圖表內
        }
    }


    // Update is called once per frame
    void Update() 
    {
        /*if (hosSpawned) 
        {
        
        }*/
        if (PlayerController.instance != null && PlayerController.instance.gameObject.activeSelf)
        {
            // 玩家存活时处理正常的击退和移动逻辑
            if (knockBackCounter > 0)
            {
                knockBackCounter -= Time.deltaTime;

                if (moveSpeed > 0)
                {
                    moveSpeed = -moveSpeed * 2f; // 击退时翻转移动方向
                }

                if (knockBackCounter <= 0)
                {
                    moveSpeed = Mathf.Abs(moveSpeed * 0.5f); // 击退结束后恢复正向速度
                }
            }

            // 如果有目标（玩家），计算方向并移动
            if (target != null)
            {
                theRB.velocity = (target.position - transform.position).normalized * moveSpeed;
            }
            else
            {
                theRB.velocity = Vector2.zero; // 没有目标时保持静止
            }

            //击中计时器逻辑
            if (hitCounter > 0f)
            {
                hitCounter -= Time.deltaTime;
            }
        }
        else
        {
            // 玩家死亡时，立即停止敌人的所有移动
            theRB.velocity = Vector2.zero;

            // 停用 AIPath 来停止敌人移动
            if (aiPath != null)
            {
                aiPath.enabled = false;  // 禁用 AIPath 组件，停止敌人移动
            }

            // 如果有动画组件，关闭动画
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = false; // 停止动画
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
        if (shouldKnockback == true)
        {
            knockBackCounter = knockBackTime;
        }
    }
    /*private void SpawnSequenceCompleted() 
    {
        Renderer.enabled = true;
        spawnIndicator
    
    }*/
}
