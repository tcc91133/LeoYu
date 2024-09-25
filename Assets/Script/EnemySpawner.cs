using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private float spawnFrequency = 2f;
    [SerializeField] private int enemiesPerSpawn = 1;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float maxSpawnDistance = 5f;

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            for (int i = 0; i < enemiesPerSpawn; i++)
            {
                SpawnEnemy();
            }
            yield return new WaitForSeconds(spawnFrequency);
        }
    }

    private void SpawnEnemy()
    {
        Vector2 spawnPosition = GetRandomSpawnPositionOutsideCamera();
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    private Vector2 GetRandomSpawnPositionOutsideCamera()
    {
        float screenHeight = 2f * mainCamera.orthographicSize;
        float screenWidth = screenHeight * mainCamera.aspect;
        Vector2 spawnPosition;

        do
        {
            spawnPosition = new Vector2(
                Random.Range(mainCamera.transform.position.x - screenWidth / 2 - maxSpawnDistance, mainCamera.transform.position.x + screenWidth / 2 + maxSpawnDistance),
                Random.Range(mainCamera.transform.position.y - screenHeight / 2 - maxSpawnDistance, mainCamera.transform.position.y + screenHeight / 2 + maxSpawnDistance)
            );
        } while (spawnPosition.x >= mainCamera.transform.position.x - screenWidth / 2 && spawnPosition.x <= mainCamera.transform.position.x + screenWidth / 2 &&
                 spawnPosition.y >= mainCamera.transform.position.y - screenHeight / 2 && spawnPosition.y <= mainCamera.transform.position.y + screenHeight / 2);

        return spawnPosition;
    }
}
