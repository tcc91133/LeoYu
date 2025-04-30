using System.Collections;
using UnityEngine;
using Pathfinding;

public class BossJumpAttack : MonoBehaviour, IBossSkill
{
    [Header("Jump Settings")]
    public AIPath aiPath;                     // A*路径组件
    public float chargeTime = 1f;             // 蓄力时间
    public float jumpSpeed = 10f;             // 跳跃速度
    public float jumpHeightOffset = 3f;       // 跳跃高度偏移
    public float hangTime = 0.5f;             // 空中悬停时间
    public float recoveryTime = 1f;           // 落地恢复时间
    public float cooldown = 5f;               // 技能冷却
    public float arcHeight = 2f;              // 抛物线高度
    public SkillCategory category = SkillCategory.Melee;
    public SkillCategory Category => category;


    [Header("Range Settings")]  // 新增專用範圍設定區塊
    public float rangeMin = 2f;               // 最小有效距離
    public float rangeMax = 5f;               // 最大有效距離
    public float weight = 1f;                 // 技能權重

    [Header("Collision Settings")]
    public Collider2D hitCollider;            // 攻击判定碰撞体
    public bool disableAllColliders = true;   // 是否禁用所有碰撞体

    [Header("Effects")]
    public GameObject landingEffectPrefab;    // 落地特效
    public float landingEffectYOffset = -1f;  // 特效Y轴偏移
    public float cameraShakeIntensity = 3f;   // 镜头震动强度

    // 状态变量
    private bool _isExecuting;
    private float _cooldownTimer;
    private Vector2 _landingPosition;
    private CameraController _cameraController;
    private Collider2D[] _allColliders;

    // ========== 接口屬性 ========== //
    public bool IsAvailable => !_isExecuting && _cooldownTimer <= 0f;
    public float Cooldown => cooldown;
    public float RangeMin => rangeMin;
    public float RangeMax => rangeMax;
    public float Weight => weight;

    void Awake()
    {
        _cameraController = FindObjectOfType<CameraController>();
        _allColliders = GetComponents<Collider2D>();
        if (hitCollider == null && _allColliders.Length > 0)
            hitCollider = _allColliders[0];
    }

    void Update()
    {
        if (_cooldownTimer > 0)
            _cooldownTimer -= Time.deltaTime;
    }

    public IEnumerator ExecuteSkill()
    {
        _isExecuting = true;
        _cooldownTimer = cooldown;
        yield return StartCoroutine(PerformJumpAttack());
    }

    private IEnumerator PerformJumpAttack()
    {
        // === 階段1：蓄力準備 ===
        aiPath.enabled = false;
        yield return new WaitForSeconds(chargeTime);

        // === 階段2：計算落點 ===
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _landingPosition = player != null ?
            player.transform.position :
            transform.position + Vector3.right * 3f;

        // 確保落點在有效範圍內
        float distance = Vector2.Distance(transform.position, _landingPosition);
        _landingPosition = Vector2.Lerp(
            transform.position,
            _landingPosition,
            Mathf.Clamp01(distance / rangeMax)
        );

        SetCollidersActive(false);

        // === 階段3：跳躍動畫 ===
        Vector2 startPos = transform.position;
        Vector2 peakPos = new Vector2(_landingPosition.x, _landingPosition.y + jumpHeightOffset);

        yield return StartCoroutine(ParabolicMovement(startPos, peakPos)); // 上升
        yield return new WaitForSeconds(hangTime); // 懸停
        yield return StartCoroutine(ParabolicMovement(transform.position, _landingPosition)); // 下降

        // === 階段4：落地效果 ===
        SetCollidersActive(true);
        SpawnLandingEffects();
        yield return new WaitForSeconds(recoveryTime);

        aiPath.enabled = true;
        _isExecuting = false;
    }

    private IEnumerator ParabolicMovement(Vector2 start, Vector2 end)
    {
        float distance = Vector2.Distance(start, end);
        float duration = distance / jumpSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float parabola = -t * t + t;
            Vector2 currentPos = Vector2.Lerp(start, end, t);
            currentPos.y += parabola * arcHeight;
            transform.position = currentPos;
            yield return null;
        }
        transform.position = end;
    }

    private void SetCollidersActive(bool active)
    {
        if (disableAllColliders)
        {
            foreach (var col in _allColliders)
                if (col != null) col.enabled = active;
        }
        else if (hitCollider != null)
        {
            hitCollider.enabled = active;
        }
    }

    private void SpawnLandingEffects()
    {
        if (landingEffectPrefab == null) return;

        Vector3 spawnPos = transform.position + new Vector3(0, landingEffectYOffset, 0);
        Instantiate(landingEffectPrefab, spawnPos, Quaternion.identity);

        if (_cameraController != null)
            _cameraController.ShakeCamera(CameraController.ShakeType.Large);
    }

    public void OnSkillFinished()
    {
        SetCollidersActive(true);
    }

    // 調試用範圍可視化
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangeMin);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangeMax);
    }
}