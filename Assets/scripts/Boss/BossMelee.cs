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
    }

    [Header("Melee Settings")]
    public AIPath aiPath;
    public List<MeleeAttackPhase> attackPhases = new List<MeleeAttackPhase>();
    public GameObject meleePrefab;
    public Animator animator;
    public string attackAnimName = "Attack";

    [Header("Skill Settings")]
    public float cooldown = 8f;
    public float rangeMin = 0f;
    public float rangeMax = 5f;
    public float weight = 5f;
    public SkillCategory category = SkillCategory.Melee;

    private float _cooldownTimer;
    private bool _isExecuting;
    private Transform _player;

    public bool IsAvailable => !_isExecuting && _cooldownTimer <= 0f;
    public float Cooldown => cooldown;
    public float RangeMin => rangeMin;
    public float RangeMax => rangeMax;
    public float Weight => weight;
    public SkillCategory Category => category;

    void Awake()
    {
        _player = GameObject.FindWithTag("Player")?.transform;
    }

    void Update()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;
    }

    public IEnumerator ExecuteSkill()
    {
        if (_isExecuting || _player == null) yield break;

        _isExecuting = true;
        _cooldownTimer = cooldown;

        aiPath.enabled = false;

        foreach (var atk in attackPhases)
        {
            if (animator) animator.Play(attackAnimName);

            yield return new WaitForSeconds(atk.chargeTime);

            Vector2 dir = (_player.position - transform.position).normalized;

            float moved = 0f;
            while (moved < atk.distance)
            {
                float step = atk.speed * Time.deltaTime;
                transform.position += (Vector3)(dir * step);
                moved += step;
                yield return null;
            }

            if (meleePrefab)
            {
                Instantiate(meleePrefab, transform.position, Quaternion.identity);
            }

            yield return new WaitForSeconds(atk.recoveryTime);
        }

        aiPath.enabled = true;
        _isExecuting = false;
    }

    public void OnSkillFinished()
    {
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
