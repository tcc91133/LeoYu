using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class BossSummonSkill : MonoBehaviour, IBossSkill
{
    [System.Serializable]
    public class SummonPrefab
    {
        public GameObject prefab;
        public int count = 1;
        [Range(0f, 1f)] public float spawnRadius = 0.9f;
    }

    [Header("Skill Settings")]
    public float castTime = 2f;
    public float cooldown = 15f;
    public Animator bossAnimator;
    public string summonAnimationName = "Summon";

    [Header("Summon Settings")]
    public List<SummonPrefab> summonPrefabs = new List<SummonPrefab>();
    public float spawnInterval = 0.3f;
    public CameraController.ShakeType summonShakeType = CameraController.ShakeType.Medium;

    [Header("Camera Bounds")]
    public Vector2 cameraMinBounds = new Vector2(-10, -5);
    public Vector2 cameraMaxBounds = new Vector2(10, 5);

    [Header("Skill Interface Settings")]
    public SkillCategory category = SkillCategory.Unrestricted;
    public float rangeMin = 0f;
    public float rangeMax = 100f;
    public float weight = 1f;

    private AIPath aiPath;
    private bool isCasting;
    private CameraController _cameraController;
    private Transform _player;

    public float Cooldown => cooldown;
    public SkillCategory Category => category;
    public float RangeMin => rangeMin;
    public float RangeMax => rangeMax;
    public float Weight => weight;
    public bool IsAvailable => !isCasting && _cooldownTimer <= 0f;
    private float _cooldownTimer;

    void Awake()
    {
        aiPath = GetComponent<AIPath>();
        _player = GameObject.FindWithTag("Player")?.transform;
        _cameraController = Camera.main?.GetComponent<CameraController>();
        if (bossAnimator == null) bossAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;
    }

    public IEnumerator ExecuteSkill()
    {
        isCasting = true;
        _cooldownTimer = cooldown;
        aiPath.enabled = false;

        if (_cameraController != null)
        {
            _cameraController.ShakeCamera(summonShakeType);
            Debug.Log("Summon skill camera shake triggered");
        }

        if (bossAnimator != null)
        {
            bossAnimator.Play(summonAnimationName);
            bossAnimator.speed = 1f;
        }

        StartCoroutine(SummonCoroutine());

        yield return new WaitForSeconds(castTime);

        if (bossAnimator != null)
            bossAnimator.Play("Idle");

        isCasting = false;
        aiPath.enabled = true;
    }

    private IEnumerator SummonCoroutine()
    {
        foreach (var summon in summonPrefabs)
        {
            for (int i = 0; i < summon.count; i++)
            {
                Vector2 spawnPos = GetValidSpawnPosition(summon.spawnRadius);
                Instantiate(summon.prefab, spawnPos, Quaternion.identity);
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }

    public void OnSkillFinished()
    {
        if (isCasting)
        {
            StopAllCoroutines();
            if (bossAnimator != null)
                bossAnimator.Play("Idle");
            aiPath.enabled = true;
            isCasting = false;
        }
    }

    private Vector2 GetValidSpawnPosition(float radiusScale)
    {
        Vector2 center = (cameraMinBounds + cameraMaxBounds) * 0.5f;
        Vector2 size = cameraMaxBounds - cameraMinBounds;
        Vector2 randomPoint = center + new Vector2(
            (Random.value - 0.5f) * size.x * radiusScale,
            (Random.value - 0.5f) * size.y * radiusScale
        );
        return randomPoint;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = (Vector3)(cameraMinBounds + cameraMaxBounds) * 0.5f;
        Vector3 size = (Vector3)(cameraMaxBounds - cameraMinBounds);
        Gizmos.DrawWireCube(center, size);
    }
}
