using UnityEngine;
using System.Collections;
using Pathfinding;

public class BossShootingSkill : MonoBehaviour, IBossSkill
{
    [Header("基本設定")]
    public float chargeTime = 1f;
    public float castDuration = 2f;
    public float recoveryTime = 1f;
    public float cooldown = 3f;
    public AIPath aiPath;

    [Header("動畫控制")]
    public Animator bossAnimator;
    public string shootAnimationName = "Shoot";
    public float animationSpeedMultiplier = 1f;

    [Header("技能參數")]
    public float rangeMin = 0f;
    public float rangeMax = 10f;
    public float weight = 1f;
    public SkillCategory category = SkillCategory.Ranged;

    [Header("發射設定")]
    public GameObject projectilePrefab;
    public int projectileCount = 5;
    public float xOffset = 1f;
    public float yOffset = 0f;
    public float prefabScale = 1f;

    private bool _isOnCooldown = false;
    private bool _isExecuting = false;
    private Transform _player;

    public bool IsAvailable => !_isOnCooldown && !_isExecuting;
    public float Cooldown => cooldown;
    public float RangeMin => rangeMin;
    public float RangeMax => rangeMax;
    public float Weight => weight;
    public SkillCategory Category => category;

    void Awake()
    {
        _player = GameObject.FindWithTag("Player")?.transform;
        if (aiPath == null)
            aiPath = GetComponent<AIPath>();

        if (bossAnimator == null)
            bossAnimator = GetComponent<Animator>();
    }

    public IEnumerator ExecuteSkill()
    {
        _isOnCooldown = true;
        _isExecuting = true;

        // 關閉AI移動
        if (aiPath != null)
            aiPath.enabled = false;

        // 1. 播放單次射擊動畫
        if (bossAnimator != null)
        {
            bossAnimator.StopPlayback();
            bossAnimator.Play(shootAnimationName, -1, 0f);

            // 計算動畫速度以匹配技能時間
            AnimatorStateInfo state = bossAnimator.GetCurrentAnimatorStateInfo(0);
            bossAnimator.speed = (state.length / (chargeTime + castDuration)) * animationSpeedMultiplier;
        }

        // 2. 蓄力階段
        if (chargeTime > 0)
            yield return new WaitForSeconds(chargeTime);

        // 3. 射擊階段
        float interval = castDuration / Mathf.Max(projectileCount, 1);
        for (int i = 0; i < projectileCount; i++)
        {
            if (projectilePrefab != null && _player != null)
            {
                Vector3 spawnPos = CalculateSpawnPosition();
                CreateProjectile(spawnPos);
            }
            yield return new WaitForSeconds(interval);
        }

        // 4. 恢復階段
        if (recoveryTime > 0)
            yield return new WaitForSeconds(recoveryTime);

        // 重置狀態
        ResetSkill();
    }

    private Vector3 CalculateSpawnPosition()
    {
        float offsetX = _player.position.x < transform.position.x ? -xOffset : xOffset;
        return transform.position + new Vector3(offsetX, yOffset, 0f);
    }

    private void CreateProjectile(Vector3 spawnPos)
    {
        GameObject obj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        obj.transform.localScale = Vector3.one * prefabScale;

        Vector2 dir = (_player.position - spawnPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        obj.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void ResetSkill()
    {
        // 重置動畫
        if (bossAnimator != null)
        {
            bossAnimator.speed = 1f;
            bossAnimator.Play("Idle");
        }

        // 重新啟用AI
        if (aiPath != null)
            aiPath.enabled = true;

        _isExecuting = false;
        StartCoroutine(CooldownRoutine());
    }

    public void OnSkillFinished()
    {
        ResetSkill();
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(cooldown);
        _isOnCooldown = false;
    }

    // 除錯用可視化
    private void OnDrawGizmosSelected()
    {
        if (_player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _player.position);
        }
    }
}