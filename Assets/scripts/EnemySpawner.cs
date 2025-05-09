﻿using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyToSpawn;
    public float timeToSpawn;
    private float spawnCounter;

    public Transform minSpawn, maxSpawn;
    private Transform target;
    private float despawnDistance;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    public int checkPerFrame;
    private int enemyToCheck;

    private bool canSpawn = true; // 控制是否可以生成敌人

    void Start()
    {
        spawnCounter = timeToSpawn;
        target = PlayerHealthController.instance.transform;
        despawnDistance = Vector3.Distance(transform.position, maxSpawn.position) + 4f;
    }

    void Update()
    {
        if (!canSpawn) return; // 如果不能生成敌人，直接退出

        spawnCounter -= Time.deltaTime;
        if (spawnCounter <= 0)
        {
            spawnCounter = timeToSpawn;

            // 生成敌人并添加到列表中
            GameObject newEnemy = Instantiate(enemyToSpawn, SelectSpawnPoint(), transform.rotation);
            spawnedEnemies.Add(newEnemy);

            transform.position = target.position;

            int checkTarget = enemyToCheck + checkPerFrame;
            while (enemyToCheck < checkTarget)
            {
                if (enemyToCheck < spawnedEnemies.Count)
                {
                    if (spawnedEnemies[enemyToCheck] != null)
                    {
                        if (Vector3.Distance(transform.position, spawnedEnemies[enemyToCheck].transform.position) > despawnDistance)
                        {
                            Destroy(spawnedEnemies[enemyToCheck]);
                            spawnedEnemies.RemoveAt(enemyToCheck);
                            checkTarget--;
                        }
                        else
                        {
                            enemyToCheck++;
                        }
                    }
                    else
                    {
                        spawnedEnemies.RemoveAt(enemyToCheck);
                        checkTarget--;
                    }
                }
                else
                {
                    enemyToCheck = 0;
                    checkTarget = 0;
                }
            }
        }
    }

    public Vector3 SelectSpawnPoint()
    {
        Vector3 spawnPoint = Vector3.zero;
        bool spawnVerticalEdge = UnityEngine.Random.Range(0f, 1f) > .5f;

        if (spawnVerticalEdge)
        {
            spawnPoint.y = UnityEngine.Random.Range(minSpawn.position.y, maxSpawn.position.y);

            if (UnityEngine.Random.Range(0f, 1f) > .5f)
            {
                spawnPoint.x = maxSpawn.position.x;
            }
            else
            {
                spawnPoint.x = minSpawn.position.x;
            }
        }
        else
        {
            spawnPoint.x = UnityEngine.Random.Range(minSpawn.position.x, maxSpawn.position.x);

            if (UnityEngine.Random.Range(0f, 1f) > .5f)
            {
                spawnPoint.y = maxSpawn.position.y;
            }
            else
            {
                spawnPoint.y = minSpawn.position.y;
            }
        }
        return spawnPoint;
    }

    // 停止生成敌人
    public void StopSpawning()
    {
        canSpawn = false;
    }
}


