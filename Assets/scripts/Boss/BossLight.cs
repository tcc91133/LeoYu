using UnityEngine;
using System.Collections;
using Pathfinding;
using UnityEngine.UI;

public class BossDarknessSkill : MonoBehaviour, IBossSkill
{
    [Header("Skill Settings")]
    public float castTime = 3f;
    public float cooldown = 20f;
    public float rangeMin = 0f;
    public float rangeMax = 15f;
    public float weight = 0.8f;
    public SkillCategory category = SkillCategory.Unrestricted;
    [Header("电池UI控制")]
    public Image batteryImage;
    [Header("Animation Control")]
    public Animator bossAnimator;
    public string darknessAnimationName = "DarknessCast";
    public float animationSpeedMultiplier = 1f;

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

    private void Awake()
    {
        _aiPath = GetComponent<AIPath>();

        // 自动获取引用
        if (sceneLighting == null)
            sceneLighting = FindObjectOfType<SceneLighting2D>();

        if (bossAnimator == null)
            bossAnimator = GetComponent<Animator>();

        // 尝试自动获取电池Image
        if (batteryImage == null && sceneLighting != null)
        {
            // 尝试通过场景灯光对象查找
            batteryImage = sceneLighting.GetComponentInChildren<Image>();
        }
    }

    void Update()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;
    }

    public IEnumerator ExecuteSkill()
    {
        _isExecuting = true;
        _cooldownTimer = cooldown;
        _aiPath.enabled = false;

        // 播放动画
        if (bossAnimator != null)
        {
            bossAnimator.Play(darknessAnimationName, -1, 0f);
            bossAnimator.speed = bossAnimator.GetCurrentAnimatorStateInfo(0).length / castTime;
        }

        // 设置电池颜色
        if (batteryImage != null)
        {
            batteryImage.color = lockedBatteryColor;
        }

        // 原有场景灯光控制
        if (sceneLighting != null)
        {
            sceneLighting.IsInCastMode = true;
            sceneLighting.LockControls(true);
        }

        yield return new WaitForSeconds(castTime);

        if (sceneLighting != null)
            sceneLighting.ForceCompleteDarken();

        ResetSkillState();
    }


    public void OnSkillFinished()
    {
        StopAllCoroutines();
        ResetSkillState();
    }

    private void ResetSkillState()
    {
        if (bossAnimator != null)
        {
            bossAnimator.speed = 1f;
            bossAnimator.Play("Idle");
        }

        if (_aiPath != null)
            _aiPath.enabled = true;

        if (sceneLighting != null)
        {
            sceneLighting.IsInCastMode = false;
            sceneLighting.UnlockControls();

            // 恢复电池颜色
            if (batteryImage != null)
                batteryImage.color = Color.white;
        }

        _isExecuting = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, rangeMax);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rangeMin);
    }

    // 动画事件调用点（可选）
  
}