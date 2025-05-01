using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossSkillManager : MonoBehaviour
{
    [Header("Settings")]
    public string playerTag = "Player";
    public float playerCheckRadius = 10f;

    [Header("Skills")]
    public List<MonoBehaviour> skillComponents = new();

    [Header("初始延遲")]
    public float initialDelay = 3f;

    [Header("Category Cooldowns")]
    public float meleeCategoryCooldown = 1f;
    public float rangedCategoryCooldown = 2f;
    public float unrestrictedCategoryCooldown = 0.5f;

    [Header("Delay After Melee or Ranged")]
    public float delayAfterMeleeOrRanged = 1f;

    private Dictionary<SkillCategory, List<IBossSkill>> _categorizedSkills = new();
    private Dictionary<SkillCategory, float> _nextAvailableCategoryTime = new();
    private IBossSkill _currentSkill;
    private SkillCategory? _lastCategory;
    private float _lastCastTime;
    private float _spawnTime;

    void Start()
    {
        _spawnTime = Time.time;

        foreach (SkillCategory category in System.Enum.GetValues(typeof(SkillCategory)))
        {
            _categorizedSkills[category] = new List<IBossSkill>();
            _nextAvailableCategoryTime[category] = 0f;
        }

        foreach (var component in skillComponents)
        {
            if (component is IBossSkill skill)
            {
                _categorizedSkills[skill.Category].Add(skill);
                Debug.Log($"註冊技能: {skill.GetType().Name} (類別: {skill.Category})");
            }
        }
    }

    void Update()
    {
        if (Time.time - _spawnTime < initialDelay) return;

        if (_currentSkill == null)
            TryCastRandomSkill();
    }

    private void TryCastRandomSkill()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null) return;

        List<IBossSkill> availableSkills = new();
        float totalWeight = 0f;
        Vector2 playerPos = player.transform.position;

        foreach (var kv in _categorizedSkills)
        {
            SkillCategory category = kv.Key;
            List<IBossSkill> skills = kv.Value;

            if (Time.time < _nextAvailableCategoryTime[category])
                continue;

            if (_lastCategory.HasValue && category != SkillCategory.Unrestricted)
            {
                bool isSameAsLast = category == _lastCategory.Value;
                bool lastWasMeleeOrRanged = _lastCategory == SkillCategory.Melee || _lastCategory == SkillCategory.Ranged;
                if (isSameAsLast && lastWasMeleeOrRanged && Time.time - _lastCastTime < delayAfterMeleeOrRanged)
                    continue;
            }

            foreach (var skill in skills)
            {
                float distance = Vector2.Distance(transform.position, playerPos);
                if (skill.IsAvailable && distance >= skill.RangeMin && distance <= skill.RangeMax && skill.Weight > 0)
                {
                    availableSkills.Add(skill);
                    totalWeight += skill.Weight;
                }
            }
        }

        if (availableSkills.Count == 0) return;

        float randomPoint = Random.Range(0f, totalWeight);
        float accumulated = 0f;

        foreach (var skill in availableSkills)
        {
            accumulated += skill.Weight;
            if (randomPoint <= accumulated)
            {
                _currentSkill = skill;
                _lastCategory = skill.Category;
                _lastCastTime = Time.time;
                _nextAvailableCategoryTime[skill.Category] = Time.time + GetCategoryCooldown(skill.Category);
                StartCoroutine(CastSkillRoutine(skill));
                break;
            }
        }
    }

    private float GetCategoryCooldown(SkillCategory category)
    {
        return category switch
        {
            SkillCategory.Melee => meleeCategoryCooldown,
            SkillCategory.Ranged => rangedCategoryCooldown,
            SkillCategory.Unrestricted => unrestrictedCategoryCooldown,
            _ => 1f,
        };
    }

    private IEnumerator CastSkillRoutine(IBossSkill skill)
    {
        Debug.Log($"執行技能: {skill.GetType().Name}");
        yield return skill.ExecuteSkill();
        skill.OnSkillFinished();
        _currentSkill = null;
        Debug.Log($"技能結束: {skill.GetType().Name}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, playerCheckRadius);
    }
}
