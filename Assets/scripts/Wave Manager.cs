using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Pathfinding;

public class WaveManager : MonoBehaviour
{
    public Transform playerTransform;

    [Header("Enemy Scaling")]
    public float healthGrowthPerWave = 1.2f;
    public float speedGrowthPerWave = 1.1f;

    [Header("Spawn Settings")]
    [SerializeField] private float minSpawnDistance = 1.0f;
    [SerializeField] private float maxSpawnDistance = 2.0f;

    [Header("Settings")]
    [SerializeField] private float waveDuration;
    private float timer;
    private bool isTimeOn;
    private int currentWaveIndex;
    private bool isFinalWave = false;

    [Header("Waves")]
    [SerializeField] private Wave[] waves;
    private List<float> localCounters = new List<float>();

    private WaveManageUI ui;
    public CompleteManager completeManager;

    private void Awake()
    {
        ui = GetComponent<WaveManageUI>();
    }

    private void Start()
    {
        StartWave(currentWaveIndex);
    }

    private void Update()
    {
        if (playerTransform == null || !playerTransform.gameObject.activeInHierarchy)
        {
            StopGeneratingEnemies();
            return;
        }

        if (!isTimeOn) return;

        if (timer < waveDuration)
        {
            ManageCurrentWave();
            ui.UpdateTimerText(((int)(waveDuration - timer)).ToString());
        }
        else
        {
            StartWaveTransition();
        }
    }

    private void StopGeneratingEnemies()
    {
        isTimeOn = false;
        timer = 0f;
    }

    private void StartWave(int waveIndex)
    {
        ui.UpdateWaveText($"波次 {waveIndex + 1}/{waves.Length}");

        localCounters.Clear();
        foreach (WaveSegment segment in waves[waveIndex].segments)
            localCounters.Add(1);

        // Spawn immediately
        foreach (WaveSegment segment in waves[waveIndex].segments)
        {
            if (segment.spawnImmediately && segment.immediateSpawnCount > 0)
            {
                for (int i = 0; i < segment.immediateSpawnCount; i++)
                {
                    SpawnEnemy(segment.prefub);
                }
            }
        }

        isTimeOn = true;
        timer = 0f;
    }

    private void ManageCurrentWave()
    {
        Wave currentWave = waves[currentWaveIndex];

        for (int i = 0; i < currentWave.segments.Count; i++)
        {
            WaveSegment segment = currentWave.segments[i];

            if (segment.spawnImmediately) continue;

            float tStart = segment.tStartEnd.x / 100 * waveDuration;
            float tEnd = segment.tStartEnd.y / 100 * waveDuration;

            if (timer < tStart || timer > tEnd) continue;

            float timeSinceSegmentStart = timer - tStart;
            float spawnDelay = 1f / segment.spawnFrequency;

            if (timeSinceSegmentStart / spawnDelay > localCounters[i])
            {
                SpawnEnemy(segment.prefub);
                localCounters[i]++;
            }
        }

        timer += Time.deltaTime;
    }

    private void SpawnEnemy(GameObject prefab)
    {
        GameObject enemyObj = Instantiate(prefab, GetSpawnPosition(), Quaternion.identity, transform);
        EnemyController enemy = enemyObj.GetComponent<EnemyController>();

        if (enemy != null)
        {
            int applyTimes = currentWaveIndex / 5;
            float healthScale = Mathf.Pow(1.5f, applyTimes);
            float speedScale = Mathf.Pow(1f, applyTimes);

            enemy.health *= healthScale;

            var aiPath = enemyObj.GetComponent<AIPath>();
            if (aiPath != null)
            {
                aiPath.canMove = true;
                aiPath.maxSpeed = Mathf.Clamp(aiPath.maxSpeed * speedScale, 1f, 10f);
            }
        }
    }

    private void StartWaveTransition()
    {
        isTimeOn = false;
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Length)
        {
            isFinalWave = true;
            ui.UpdateTimerText("");
            ui.UpdateWaveText("波次完成，擊敗boss");

            if (completeManager != null)
            {
                completeManager.CheckEnemiesCleared(isFinalWave);
            }
        }
        else
        {
            StartWave(currentWaveIndex);
        }
    }

    private Vector2 GetSpawnPosition()
    {
        Camera cam = Camera.main;
        if (cam == null) return playerTransform.position;

        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float mapMinX = -22f, mapMaxX = 22f;
        float mapMinY = -22f, mapMaxY = 22f;

        Vector2 spawnPos;
        int maxAttempts = 10;

        do
        {
            Vector2 direction = Random.insideUnitCircle.normalized;
            float spawnDistance = Random.Range(minSpawnDistance, maxSpawnDistance) * Mathf.Max(camWidth, camHeight);
            spawnPos = (Vector2)cam.transform.position + direction * spawnDistance;

            spawnPos.x = Mathf.Clamp(spawnPos.x, mapMinX, mapMaxX);
            spawnPos.y = Mathf.Clamp(spawnPos.y, mapMinY, mapMaxY);
        } while (maxAttempts-- > 0 && (IsPositionInCameraView(spawnPos) || Physics2D.OverlapCircle(spawnPos, 0.5f) != null));

        return spawnPos;
    }

    private bool IsPositionInCameraView(Vector2 position)
    {
        Camera cam = Camera.main;
        if (cam == null) return false;

        Vector3 viewportPoint = cam.WorldToViewportPoint(position);
        return viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1;
    }
}

[System.Serializable]
public struct Wave
{
    public string name;
    public List<WaveSegment> segments;
}

[System.Serializable]
public struct WaveSegment
{
    [MinMaxSlider(0, 100)] public Vector2 tStartEnd;
    public float spawnFrequency;
    public GameObject prefub;

    public bool spawnImmediately;
    public int immediateSpawnCount;
}
