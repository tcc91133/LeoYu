﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class WaveManager : MonoBehaviour
{
    public Transform playerTransform;

    private WaveManageUI ui;

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
        if (playerTransform != null)
        {
            Vector3 playerPosition = playerTransform.position;
            //Debug.Log("玩家位置：" + playerPosition);
        }

        if (!isTimeOn)
        {
            return;
        }

        if (timer < waveDuration) 
        {
            manageCurrentWave();

            string timerSrting = ((int)(waveDuration - timer)).ToString();
            ui.UpdateTimerText(timerSrting);
        }
            
        else
            StartWaveTransition();
    }
    private void StarWave(int waveIndex)
    {
        ui.UpdateWaveText("Wave" + (currentWaveIndex + 1) + "/" + waves.Length);

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
        isTimeOn = false;
        DefeatAllEnemies();
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Length)
        { 
            ui.UpdateTimerText("");
            ui.UpdateWaveText("通關");
        }
           
        else
            StarWave(currentWaveIndex);
    }

    private void DefeatAllEnemies()
    {
        while (transform.childCount > 0)
        {
            Transform child = transform.transform.GetChild(0);
            child.SetParent(null);
            Object.Destroy(child.gameObject);
        }
    }

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
