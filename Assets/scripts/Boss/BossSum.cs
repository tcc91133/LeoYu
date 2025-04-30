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
    public string castAnimationName = "Cast";

    public float Cooldown => cooldown;

    [Header("Summon Settings")]
    public List<SummonPrefab> summonPrefabs = new List<SummonPrefab>();
    public float spawnPadding = 0.5f;

    [Header("Camera Bounds")]
    public Vector2 cameraMinBounds = new Vector2(-10, -5);
    public Vector2 cameraMaxBounds = new Vector2(10, 5);

    [Header("Skill Interface Settings")]
    public SkillCategory category = SkillCategory.Unrestricted;
    public float rangeMin = 0f;
    public float rangeMax = 100f;
    public float weight = 1f;

    private AIPath aiPath;
    private Camera mainCamera;
    private bool isFinished;

    void Start()
    {
        aiPath = GetComponent<AIPath>();
        mainCamera = Camera.main;

        if (TryGetCameraBoundsFromController(out var min, out var max))
        {
            cameraMinBounds = min;
            cameraMaxBounds = max;
        }
    }

    public SkillCategory Category => category;
    public float RangeMin => rangeMin;
    public float RangeMax => rangeMax;
    public float Weight => weight;
    public bool IsAvailable => true;

    public IEnumerator ExecuteSkill()
    {
        isFinished = false;
        aiPath.enabled = false;

        TriggerSmallCameraShake();

        if (bossAnimator != null)
        {
            bossAnimator.Play(castAnimationName);
        }

        yield return new WaitForSeconds(castTime);

        foreach (var summon in summonPrefabs)
        {
            for (int i = 0; i < summon.count; i++)
            {
                Vector2 spawnPos = GetValidSpawnPosition(summon.spawnRadius);
                Instantiate(summon.prefab, spawnPos, Quaternion.identity);
                yield return new WaitForSeconds(spawnPadding);
            }
        }

        aiPath.enabled = true;
        isFinished = true;
    }

    public void OnSkillFinished() { }

    private void TriggerSmallCameraShake()
    {
        var controller = FindObjectOfType<CameraController>();
        if (controller != null)
        {
            controller.ShakeCamera(CameraController.ShakeType.Small);
        }
    }

    private Vector2 GetValidSpawnPosition(float radiusScale)
    {
        Vector2 center = (cameraMinBounds + cameraMaxBounds) * 0.5f;
        Vector2 scaledMin = center + (cameraMinBounds - center) * radiusScale;
        Vector2 scaledMax = center + (cameraMaxBounds - center) * radiusScale;

        int side = Random.Range(0, 4);
        return side switch
        {
            0 => new Vector2(Random.Range(scaledMin.x, scaledMax.x), scaledMax.y),
            1 => new Vector2(scaledMax.x, Random.Range(scaledMin.y, scaledMax.y)),
            2 => new Vector2(Random.Range(scaledMin.x, scaledMax.x), scaledMin.y),
            _ => new Vector2(scaledMin.x, Random.Range(scaledMin.y, scaledMax.y)),
        };
    }

    private bool TryGetCameraBoundsFromController(out Vector2 min, out Vector2 max)
    {
        var controller = FindObjectOfType<CameraController>();
        if (controller != null)
        {
            min = controller.minLimit;
            max = controller.maxLimit;
            return true;
        }

        min = cameraMinBounds;
        max = cameraMaxBounds;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = (Vector3)(cameraMinBounds + cameraMaxBounds) * 0.5f;
        Vector3 size = (Vector3)(cameraMaxBounds - cameraMinBounds);
        Gizmos.DrawWireCube(center, size);

        foreach (var summon in summonPrefabs)
        {
            Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
            Vector2 scaledMin = (Vector2)center + (cameraMinBounds - (Vector2)center) * summon.spawnRadius;
            Vector2 scaledMax = (Vector2)center + (cameraMaxBounds - (Vector2)center) * summon.spawnRadius;
            Vector2 scaledCenter = (scaledMin + scaledMax) * 0.5f;
            Vector2 scaledSize = scaledMax - scaledMin;
            Gizmos.DrawWireCube(scaledCenter, scaledSize);
        }
    }
}
