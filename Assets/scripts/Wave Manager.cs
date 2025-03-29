using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class WaveManager : MonoBehaviour
{
    public Transform playerTransform;

    private WaveManageUI ui;

    public CompleteManager completeManager;
    private bool isFinalWave = false;
    [Header("Settings")]
    [SerializeField] private float waveDuration;
    private float timer;
    private bool isTimeOn;
    private int currentWaveIndex;

    [Header("Waves")]
    [SerializeField] private Wave[] waves;
    private List<float> localCounters = new List<float>();

    private void Awake()
    {
        ui = GetComponent<WaveManageUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StarWave(currentWaveIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform == null || !playerTransform.gameObject.activeInHierarchy)
        {
            StopGeneratingEnemies();
            return;
        }

        if (!isTimeOn)
        {
            return;
        }

        if (timer < waveDuration)
        {
            manageCurrentWave();

            string timerString = ((int)(waveDuration - timer)).ToString();
            ui.UpdateTimerText(timerString);
        }
        else
        {
            StartWaveTransition();
        }
    }
    private void StopGeneratingEnemies()
    {
        isTimeOn = false; // 停止计时器
        timer = 0f; // 重置计时器
    }
    private void StarWave(int waveIndex)
    {
        ui.UpdateWaveText("波次" + (currentWaveIndex + 1) + "/" + waves.Length);

        localCounters.Clear();
        foreach (WaveSegment segment in waves[waveIndex].segments)
            localCounters.Add(1);

        isTimeOn = true;
        timer = 0f;
    }
    private void manageCurrentWave()
    {
        Wave CurrentWave = waves[currentWaveIndex];
        for (int i = 0; i < CurrentWave.segments.Count; i++)
        {
            WaveSegment segment = CurrentWave.segments[i];
            float tStart = segment.tStaretEnd.x / 100 * waveDuration;
            float tEnd = segment.tStaretEnd.y / 100 * waveDuration;

            if (timer < tStart || timer > tEnd)
                continue;
            float timeSinceSegmentSrart = timer - tStart;
            float spawnDelay = 1f / segment.spawnFrequency;

            if (timeSinceSegmentSrart / spawnDelay > localCounters[i])
            {
                Instantiate(segment.prefub, GetSpawnPosition(), Quaternion.identity, transform);
                localCounters[i]++;
            }
        }
        timer += Time.deltaTime;
    }

    private void StartWaveTransition()
    {
        isTimeOn = false; // 停止计时
        currentWaveIndex++; // 进入下一个波次

        if (currentWaveIndex >= waves.Length) // 如果是最后一波
        {
            isFinalWave = true; // 设置标志，表示现在是最后一波
            ui.UpdateTimerText(""); // 清空时间显示
            ui.UpdateWaveText("波次完成，擊敗所有敵人"); // 显示通关UI

            // 通知 CompleteManager 检查敌人是否消失
            if (completeManager != null)
            {
                completeManager.CheckEnemiesCleared(isFinalWave); // 传递是否为最后一波
            }
        }
        else
        {
            StarWave(currentWaveIndex); // 启动下一个波次
        }
    }



    private Vector2 GetSpawnPosition()
    {
        Camera cam = Camera.main;
        if (cam == null) return playerTransform.position; // 防止相机为空

        // 计算相机视野的边界
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        // 设置地图边界（假设地图是固定大小）
        float mapMinX = -22f, mapMaxX = 22f;
        float mapMinY = -22f, mapMaxY = 22f;

        Vector2 spawnPos;
        int maxAttempts = 10; // 限制尝试次数，防止死循环

        do
        {
            // 计算随机方向并确保在相机外部
            Vector2 direction = Random.insideUnitCircle.normalized;
            float spawnDistance = Random.Range(1.2f, 1.5f) * Mathf.Max(camWidth, camHeight); // 保证在相机外

            // 计算生成位置
            spawnPos = (Vector2)cam.transform.position + direction * spawnDistance;

            // 限制位置在地图范围内
            spawnPos.x = Mathf.Clamp(spawnPos.x, mapMinX, mapMaxX);
            spawnPos.y = Mathf.Clamp(spawnPos.y, mapMinY, mapMaxY);

            maxAttempts--;
        } while (maxAttempts > 0 && IsPositionInCameraView(spawnPos)); // 确保位置不在相机内

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
    [MinMaxSlider(0, 100)] public Vector2 tStaretEnd;
    public float spawnFrequency;
    public GameObject prefub;
}
