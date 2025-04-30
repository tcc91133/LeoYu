using UnityEngine;
using System.Collections;
using Pathfinding;

public class BossDarknessSkill : MonoBehaviour, IBossSkill
{
    [Header("Skill Settings")]
    public float castTime = 3f;
    public float cooldown = 20f;
    public float rangeMin = 0f;
    public float rangeMax = 15f;
    public float weight = 0.8f;
    public SkillCategory category = SkillCategory.Unrestricted;

    [Header("References")]
    public SceneLighting2D sceneLighting;
    public Color lockedBatteryColor = Color.red;

    // 状态变量
    private bool _isExecuting;
    private float _cooldownTimer;
    private AIPath _aiPath;
    private float _originalBatteryTime;

    // ========== 接口实现 ========== //
    public SkillCategory Category => category;
    public bool IsAvailable => !_isExecuting && _cooldownTimer <= 0f;
    public float Cooldown => cooldown;
    public float RangeMin => rangeMin;
    public float RangeMax => rangeMax;
    public float Weight => weight;

    void Awake()
    {
        _aiPath = GetComponent<AIPath>();
        if (sceneLighting == null)
            sceneLighting = FindObjectOfType<SceneLighting2D>();
    }

    void Update()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;
    }

    public IEnumerator ExecuteSkill()
    {
        _isExecuting = true;

        // 阶段1：准备施法
        _aiPath.enabled = false;

        // 设置Cast模式并保存当前电池状态
        if (sceneLighting != null)
        {
            _originalBatteryTime = sceneLighting.BatteryTimer;
            sceneLighting.IsInCastMode = true;
            sceneLighting.LockControls(true);
        }

        // 蓄力等待
        yield return new WaitForSeconds(castTime);

        // 阶段2：立即触发关灯
        if (sceneLighting != null)
            sceneLighting.ForceCompleteDarken();

        // 阶段3：开始冷却
        _cooldownTimer = cooldown;

        // 阶段4：恢复
        _aiPath.enabled = true;
        _isExecuting = false;

        // 解除Cast模式和锁定
        if (sceneLighting != null)
        {
            sceneLighting.IsInCastMode = false;
            sceneLighting.UnlockControls();
        }
    }

    public void OnSkillFinished()
    {
        StopAllCoroutines();
        _isExecuting = false;

        if (_aiPath != null)
            _aiPath.enabled = true;

        if (sceneLighting != null)
        {
            sceneLighting.IsInCastMode = false;
            sceneLighting.UnlockControls();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, rangeMax);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangeMin);
    }
}