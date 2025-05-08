using System.Collections;
using UnityEngine;
using Pathfinding;

public class BossJumpAttack : MonoBehaviour, IBossSkill
{
    [Header("Jump Settings")]
    public AIPath aiPath;
    public float chargeTime = 1f;
    public float jumpSpeed = 10f;
    public float jumpHeightOffset = 3f;
    public float hangTime = 0.5f;
    public float recoveryTime = 1f;
    public float cooldown = 5f;
    public float arcHeight = 2f;

    [Header("Range Settings")]
    public float rangeMin = 2f;
    public float rangeMax = 5f;
    public float weight = 1f;

    [Header("Collision Settings")]
    public Collider2D hitCollider;
    public bool disableAllColliders = true;

    [Header("Effects")]
    public GameObject landingEffectPrefab;
    public float landingEffectYOffset = -1f;
    public float cameraShakeIntensity = 3f;

    [Header("Animation")]
    public Animator animator;
    public string walkingParam = "IsWalking";
    public string chargingParam = "IsCharging";
    public string jumpingParam = "IsJumping";
    public string landingTrigger = "Landing";

    private bool _isExecuting;
    private float _cooldownTimer;
    private Vector2 _landingPosition;
    private CameraController _cameraController;
    private Collider2D[] _allColliders;

    public SkillCategory category = SkillCategory.Melee;
    public SkillCategory Category => category;
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

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (_cooldownTimer > 0)
            _cooldownTimer -= Time.deltaTime;

        // 走路動畫控制（非技能狀態時）
        if (!_isExecuting && animator != null && aiPath != null)
        {
            bool isMoving = aiPath.enabled && aiPath.velocity.magnitude > 0.1f;
            animator.SetBool(walkingParam, isMoving);

            if (isMoving)
            {
                animator.SetBool(chargingParam, false);
                animator.SetBool(jumpingParam, false);
            }
        }
    }

    public IEnumerator ExecuteSkill()
    {
        _isExecuting = true;
        _cooldownTimer = cooldown;

        // 階段1：蓄力
        aiPath.enabled = false;
        if (animator != null)
        {
            animator.SetBool(walkingParam, false);
            animator.SetBool(chargingParam, true);
        }
        yield return new WaitForSeconds(chargeTime);

        // 階段2：決定落點
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _landingPosition = player != null ? player.transform.position : transform.position + Vector3.right * 3f;

        float distance = Vector2.Distance(transform.position, _landingPosition);
        _landingPosition = Vector2.Lerp(transform.position, _landingPosition, Mathf.Clamp01(distance / rangeMax));

        Vector2 startPos = transform.position;
        Vector2 peakPos = new Vector2(_landingPosition.x, _landingPosition.y + jumpHeightOffset);

        // 階段3：跳躍動畫 + 位移
        if (animator != null)
        {
            animator.SetBool(chargingParam, false);
            animator.SetBool(jumpingParam, true);
        }

        yield return StartCoroutine(ParabolicMovement(startPos, peakPos));
        yield return new WaitForSeconds(hangTime);
        yield return StartCoroutine(ParabolicMovement(transform.position, _landingPosition));

        // 落地：開啟碰撞體、觸發特效
        SetCollidersActive(true);
        SpawnLandingEffects();

        if (animator != null)
        {
            animator.SetBool(jumpingParam, false);
            animator.SetTrigger(landingTrigger);
        }

        yield return new WaitForSeconds(recoveryTime);
        aiPath.enabled = true;
        _isExecuting = false;

        // 技能執行完畢，呼叫 OnSkillFinished
        OnSkillFinished();
    }

    // 實作 IBossSkill 介面要求的方法
    public void OnSkillFinished()
    {
        // 這裡可以加入技能結束後的處理邏輯
        // 例如觸發事件、通知 Boss 狀態機等
    }

    private IEnumerator ParabolicMovement(Vector2 from, Vector2 to)
    {
        float time = 0f;
        float duration = Vector2.Distance(from, to) / jumpSpeed;
        SetCollidersActive(false);

        while (time < duration)
        {
            float t = time / duration;
            float height = Mathf.Sin(Mathf.PI * t) * arcHeight;
            transform.position = Vector2.Lerp(from, to, t) + Vector2.up * height;
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = to;
    }

    private void SpawnLandingEffects()
    {
        if (landingEffectPrefab != null)
        {
            Vector3 effectPos = transform.position + Vector3.up * landingEffectYOffset;
            Instantiate(landingEffectPrefab, effectPos, Quaternion.identity);
        }

        if (_cameraController != null)
            _cameraController.ShakeCamera(CameraController.ShakeType.Large);
    }

    private void SetCollidersActive(bool active)
    {
        if (!disableAllColliders) return;

        foreach (var col in _allColliders)
            col.enabled = active;

        if (hitCollider != null)
            hitCollider.enabled = active;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangeMin);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangeMax);
    }
}