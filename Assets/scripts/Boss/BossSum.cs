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
    public string summonAnimationName = "Summon"; // 单一动画名称

    public float Cooldown => cooldown;

    [Header("Summon Settings")]
    public List<SummonPrefab> summonPrefabs = new List<SummonPrefab>();
    public float spawnInterval = 0.3f;

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

    void Awake()
    {
        aiPath = GetComponent<AIPath>();
        if (bossAnimator == null)
            bossAnimator = GetComponent<Animator>();
    }

    public SkillCategory Category => category;
    public float RangeMin => rangeMin;
    public float RangeMax => rangeMax;
    public float Weight => weight;
    public bool IsAvailable => !isCasting;

    public IEnumerator ExecuteSkill()
    {
        isCasting = true;
        aiPath.enabled = false;

        // 1. 动画系统重置（安全版）
        if (bossAnimator != null)
        {
            bossAnimator.Rebind();
            yield return null; // 确保重置完成
            bossAnimator.Play("EmptyState", 0, 0); // 过渡空状态
            yield return null;

            // 精确控制动画播放
            AnimatorStateInfo state = bossAnimator.GetCurrentAnimatorStateInfo(0);
            float speedMultiplier = state.length / castTime;
            bossAnimator.Play(summonAnimationName, 0, 0f);
            bossAnimator.speed = speedMultiplier;

            Debug.Log($"动画长度: {state.length} 速度系数: {speedMultiplier}");
        }

        // 2. 生成位置预检测
        List<Vector2> validSpawnPositions = new List<Vector2>();
        foreach (var summon in summonPrefabs)
        {
            for (int i = 0; i < summon.count; i++)
            {
                Vector2 spawnPos = GetValidSpawnPosition(summon.spawnRadius);
                if (!Physics2D.OverlapCircle(spawnPos, 0.5f))
                {
                    validSpawnPositions.Add(spawnPos);
                    Debug.DrawRay(spawnPos, Vector2.up, Color.green, 2f);
                }
            }
        }

        // 3. 同步动画与生成
        float timer = 0;
        int spawnIndex = 0;
        while (timer < castTime)
        {
            timer += Time.deltaTime;

            // 按进度生成敌人
            if (spawnIndex < validSpawnPositions.Count &&
                timer >= (castTime * spawnIndex / validSpawnPositions.Count))
            {
                Instantiate(
                    summonPrefabs[0].prefab, // 假设使用第一个预制件
                    validSpawnPositions[spawnIndex],
                    Quaternion.identity
                );
                spawnIndex++;
            }

            yield return null;
        }

        // 4. 强制状态重置
        if (bossAnimator != null)
        {
            bossAnimator.speed = 1;
            bossAnimator.Play("Idle", 0, 0.9f); // 直接跳到动画末尾
        }

        isCasting = false;
        aiPath.enabled = true;
    }

    public void OnSkillFinished()
    {
        if (isCasting)
        {
            if (bossAnimator != null)
                bossAnimator.Play("Idle"); // 强制回到待机状态
            aiPath.enabled = true;
            isCasting = false;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = (Vector3)(cameraMinBounds + cameraMaxBounds) * 0.5f;
        Vector3 size = (Vector3)(cameraMaxBounds - cameraMinBounds);
        Gizmos.DrawWireCube(center, size);
    }
}