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
    public SkillCategory category = SkillCategory.Melee;
    public SkillCategory Category => category;

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
    public string fallingParam = "IsFalling";
    public string landingTrigger = "Landing";

    // 狀態變量
    private bool _isExecuting;
    private float _cooldownTimer;
    private Vector2 _landingPosition;
    private CameraController _cameraController;
    private Collider2D[] _allColliders;

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

        // 只在非技能執行狀態更新走路動畫
        if (!_isExecuting && animator != null && aiPath != null)
        {
            bool isMoving = aiPath.enabled && aiPath.velocity.magnitude > 0.1f;
            animator.SetBool(walkingParam, isMoving);

            // 確保其他動畫參數被重置
            if (isMoving)
            {
                animator.SetBool(chargingParam, false);
                animator.SetBool(jumpingParam, false);
                animator.SetBool(fallingParam, false);
            }
        }
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
        if (animator != null)
        {
            animator.SetBool(walkingParam, false);
            animator.SetBool(chargingParam, true);
        }
        yield return new WaitForSeconds(chargeTime);

        // === 階段2：計算落點 ===
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _landingPosition = player != null ?
            player.transform.position :
            transform.position + Vector3.right * 3f;

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

        // 起跳動畫
        if (animator != null)
        {
            animator.SetBool(chargingParam, false);
            animator.SetBool(jumpingParam, true);
        }

        yield return StartCoroutine(ParabolicMovement(startPos, peakPos)); // 上升

        // 切換到降落動畫
        if (animator != null)
        {
            animator.SetBool(jumpingParam, false);
            animator.SetBool(fallingParam, true);
        }

        yield return new WaitForSeconds(hangTime); // 懸停
        yield return StartCoroutine(ParabolicMovement(transform.position, _landingPosition)); // 下降

        // === 階段4：落地效果 ===
        SetCollidersActive(true);
        SpawnLandingEffects();

        // 觸發著陸動畫
        if (animator != null)
        {
            animator.SetBool(fallingParam, false);
            animator.SetTrigger(landingTrigger);
        }

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangeMin);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangeMax);
    }
}