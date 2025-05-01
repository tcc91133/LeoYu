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
    public AIPath aiPath; // 新增 AIPath 參考

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
    private bool _isExecuting = false; // 新增執行狀態標記
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
        if (aiPath == null) // 自動獲取如果未設定
            aiPath = GetComponent<AIPath>();
    }

    public IEnumerator ExecuteSkill()
    {
        _isOnCooldown = true;
        _isExecuting = true;
        
        // 關閉 AI 移動
        if (aiPath != null)
            aiPath.enabled = false;

        if (chargeTime > 0)
            yield return new WaitForSeconds(chargeTime);

        float interval = castDuration / Mathf.Max(projectileCount, 1);

        for (int i = 0; i < projectileCount; i++)
        {
            if (projectilePrefab != null && _player != null)
            {
                float offsetX = xOffset;
                if (_player.position.x < transform.position.x)
                    offsetX *= -1f;

                Vector3 spawnPos = transform.position + new Vector3(offsetX, yOffset, 0f);
                GameObject obj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

                obj.transform.localScale = new Vector3(prefabScale, prefabScale, 1f);

                Vector2 dir = (_player.position - spawnPos).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                obj.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }

            yield return new WaitForSeconds(interval);
        }

        if (recoveryTime > 0)
            yield return new WaitForSeconds(recoveryTime);

        // 重新啟用 AI 移動
        if (aiPath != null)
            aiPath.enabled = true;

        _isExecuting = false;
    }

    public void OnSkillFinished()
    {
        // 確保 AI 被重新啟用
        if (aiPath != null)
            aiPath.enabled = true;

        _isExecuting = false;
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(cooldown);
        _isOnCooldown = false;
    }
}