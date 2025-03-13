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

    /* private void DefeatAllEnemies()
     {
         while (transform.childCount > 0)
         {
             Transform child = transform.transform.GetChild(0);
             child.SetParent(null);
             Object.Destroy(child.gameObject);
         }
     }*/

    private Vector2 GetSpawnPosition()
    {
        Vector2 direction = Random.onUnitSphere;
        Vector2 offset = direction.normalized * Random.Range(6, 10);
        Vector2 targetPosition = (Vector2)playerTransform.transform.position + offset;

        targetPosition.x = Mathf.Clamp(targetPosition.x, -22, 22);
        targetPosition.y = Mathf.Clamp(targetPosition.y, -22, 22);

        return targetPosition;
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