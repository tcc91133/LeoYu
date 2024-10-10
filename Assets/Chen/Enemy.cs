using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pathfinding;

public class Enemy: Character
{

    public UnityEvent<Vector2> OnMovementInput;
    public UnityEvent OnAttack;

    /*[SerializeField]
    private Transform player;*/
    private Transform player;

    [SerializeField] private float chaseDistance = 3f;//追擊距離
    [SerializeField] private float attackDistance = 0.8f;//攻擊距離

    private Seeker seeker;
    private List<Vector3> pathPointList;//路徑點列表
    private int currentIndex = 0;//路徑點的索引
    private float pathGenerateInterval = 0.5f; //每0.5秒生成一次路徑
    private float pathGenerateTimer = 0f;//計時器

    [Header("攻擊")]
    public float meleeAttackDamage;//攻擊傷害
    public LayerMask playerLayer;//玩家圖層
    public float AttackCooldownDuration = 2f;//冷卻時間

    private bool isAttack = true;

    private SpriteRenderer sr;

    void Start()
    {
        EnemyManager.Instance.enemyCount++;
        // 通过标签查找玩家对象
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // 如果找不到玩家，输出警告
        if (player == null)
        {
            Debug.LogWarning("Player not found!");
        }
    }
    private void Awake()
    {
        seeker = GetComponent<Seeker>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (player == null)
            return;

        float distance = Vector2.Distance(player.position, transform.position);

        if (distance < chaseDistance)
        {
            AutoPath();

            //判斷路徑點是否為空
            if (pathPointList == null)
                return;

            if (distance <= attackDistance)//是否處於攻擊範圍
            {
                //攻擊玩家
                OnMovementInput?.Invoke(Vector2.zero);//停止移動
                if (isAttack)
                {
                    isAttack = false;
                    OnAttack?.Invoke();
                    //StartCoroutine(nameof(AttackCooldownCoroutine));
                }

                //人物翻轉
                float x = player.position.x - transform.position.x;
                if (x > 0)
                {
                    sr.flipX = true;
                }
                else
                {
                    sr.flipX = false;
                }
            }
            else
            {
                //追逐玩家
                //Vector2 direction = player.position - transform.position;
                Vector2 direction = (pathPointList[currentIndex] - transform.position).normalized;
                OnMovementInput?.Invoke(direction); //把移動方向傳給EnemyController
            }
        }
        else
        {
            //放棄追擊
            OnMovementInput?.Invoke(Vector2.zero);//停止移動
        }
    }
    //自動巡路
    private void AutoPath()
    {
        pathGenerateTimer += Time.deltaTime;

        //間隔一定時間來獲取路徑點
        if (pathGenerateTimer >= pathGenerateInterval && player!=null)
        {
            GeneratePath(player.position);
            pathGenerateTimer = 0;//重製計時器
        }


        //當路徑點為空時，計算路徑
        if (pathPointList == null || pathPointList.Count <= 0)
        {
            GeneratePath(player.position);
        }//當敵人到達當前路徑點時，遞增索引currentIndex並計算路徑
        else if (Vector2.Distance(transform.position, pathPointList[currentIndex]) <= 0.1f)
        {
            currentIndex++;
            if (currentIndex >= pathPointList.Count)
                GeneratePath(player.position);
        }
    }

    //獲取路徑點
    private void GeneratePath(Vector3 target)
    {
        currentIndex = 0;
        //三個函數起點終點回調函數
        seeker.StartPath(transform.position, target, Path =>
        {
            pathPointList = Path.vectorPath;//Path.vectorPath包含了從起點到終點的完整路徑
        });
    }
    //敵人進戰攻擊
    private void MeleeAttackAnimEvent()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackDistance, playerLayer);

        foreach (Collider2D hitCollider in hitColliders)
        {
            hitCollider.GetComponent<Character>().TakeDamage(meleeAttackDamage);
        }
    }
    //攻擊冷卻時間
    IEnumerator AttackCooldownCoroutine()
    {
        yield return new WaitForSeconds(AttackCooldownDuration);
        isAttack = true;
    }


    private void OnDrawGizmosSelected()
    {
        //顯示攻擊範圍
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        //顯示追擊範圍
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }




    //敵人波數生成
    /*private void Start()
    {
        EnemyManager.Instance.enemyCount++;
    }*/
    private void OnDestroy()
    {
        EnemyManager.Instance.enemyCount--;
    }
}