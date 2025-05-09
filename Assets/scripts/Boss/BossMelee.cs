using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BossMeleeSkill : MonoBehaviour, IBossSkill
{
    [System.Serializable]
    public class MeleeAttackPhase
    {
        public float chargeTime = 0.5f;
        public float speed = 8f;
        public float distance = 3f;
        public float recoveryTime = 0.5f;
        public float xOffset = 1f;
        public float yOffset = 0f;
        public float prefabScale = 1f;
        public CameraController.ShakeType shakeType = CameraController.ShakeType.Medium;
        public string attackStateName = "Attack"; // 使用狀態名稱而非觸發器
    }

    [Header("Melee Settings")]
    public AIPath aiPath;
    public List<MeleeAttackPhase> attackPhases = new List<MeleeAttackPhase>();
    public GameObject meleePrefab;
    public Animator animator;

    [Header("Animation States")]
    public string chargeStateName = "Charge";
    public string recoveryStateName = "Recovery";
    public string idleStateName = "Idle";
    public float animationTransitionDuration = 0.1f;

    [Header("Skill Settings")]
    public float cooldown = 8f;
    public float rangeMin = 0f;
    public float rangeMax = 5f;
    public float weight = 5f;
    public SkillCategory category = SkillCategory.Melee;

    private float _cooldownTimer;
    private bool _isExecuting;
    private Transform _player;
    private CameraController _cameraController;
    private Coroutine _currentAttackRoutine;

    public bool IsAvailable => !_isExecuting && _cooldownTimer <= 0f;
    public float Cooldown => cooldown;
    public float RangeMin => rangeMin;
    public float RangeMax => rangeMax;
    public float Weight => weight;
    public SkillCategory Category => category;

    void Awake()
    {
        _player = GameObject.FindWithTag("Player")?.transform;
        _cameraController = Camera.main?.GetComponent<CameraController>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;
    }

    public IEnumerator ExecuteSkill()
    {
        if (_isExecuting || _player == null || animator == null)
        {
            Debug.LogWarning("Cannot execute skill: condition not met");
            yield break;
        }

        _isExecuting = true;
        _cooldownTimer = cooldown;
        if (aiPath != null) aiPath.enabled = false;

        try
        {
            foreach (var atk in attackPhases)
            {
                Debug.Log($"Starting charge phase: {atk.chargeTime}s");

                // 使用 CrossFade 而不是 Play
                animator.CrossFade(chargeStateName, animationTransitionDuration);
                yield return new WaitForSeconds(atk.chargeTime);

                Debug.Log($"Starting attack phase: {atk.attackStateName}");
                animator.CrossFade(atk.attackStateName, animationTransitionDuration);

                // 衝刺移動
                Vector2 dir = (_player.position - transform.position).normalized;
                float moved = 0f;
                while (moved < atk.distance)
                {
                    float step = atk.speed * Time.deltaTime;
                    transform.position += (Vector3)(dir * step);
                    moved += step;
                    yield return null;
                }

                // 生成攻擊效果
                if (meleePrefab != null)
                {
                    float xOffset = atk.xOffset * Mathf.Sign(_player.position.x - transform.position.x);
                    Vector3 spawnPos = transform.position + new Vector3(xOffset, atk.yOffset, 0f);
                    Instantiate(meleePrefab, spawnPos, Quaternion.identity).transform.localScale = Vector3.one * atk.prefabScale;
                    if (_cameraController != null) _cameraController.ShakeCamera(atk.shakeType);
                }

                Debug.Log($"Starting recovery phase: {atk.recoveryTime}s");
                animator.CrossFade(recoveryStateName, animationTransitionDuration);
                yield return new WaitForSeconds(atk.recoveryTime);
            }
        }
        finally
        {
            Debug.Log("Skill execution completed");
            animator.CrossFade(idleStateName, animationTransitionDuration);
            if (aiPath != null) aiPath.enabled = true;
            _isExecuting = false;
        }
    }

    public void OnSkillFinished()
    {
        if (_currentAttackRoutine != null)
        {
            StopCoroutine(_currentAttackRoutine);
            _currentAttackRoutine = null;
        }

        if (animator != null)
        {
            animator.Play(idleStateName, -1, 0f);
        }

        _isExecuting = false;
        aiPath.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rangeMin);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangeMax);
    }
}