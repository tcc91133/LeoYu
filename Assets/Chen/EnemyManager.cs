using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{ 
    //��ҼҦ�
    public static EnemyManager Instance { get; private set; }

    [Header("�ĤH��s�I")]
    public Transform[] spawnPoints;

    //[Header("����Ѳ�ߵ�")]
    //public Transform[] patrolPoints;

    [Header("���d�ĤH")]
    public List<EnemyWave> enemyWaves;

    public int currentWaveIndex = 0; //���e�i�ƪ�����

    public int enemyCount = 0;      //�ĤH�Ѿl�ƶq

    //�ж��Ƿ�Ϊ���һ��
    public bool GetLastWave() => currentWaveIndex == enemyWaves.Count;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (enemyCount == 0)//���e�i�ƼĤH�����`�A�}�l�U�@�i
        {
            StartCoroutine(nameof(startNextWaveCoroutine));
        }
    }

    IEnumerator startNextWaveCoroutine()
    {
        if (currentWaveIndex >= enemyWaves.Count)
            yield break;    //�S���i�ơA�����h�X��{

        List<EnemyData> enemies = enemyWaves[currentWaveIndex].enemies;//������e�i�ƪ��ĤH�C��

        foreach (EnemyData enemyData in enemies)
        {
            for (int i = 0; i < enemyData.waveEnemyCount; i++)
            {
                GameObject enemy = Instantiate(enemyData.enemyPrefab, GetRandomSpawnPoint(), Quaternion.identity);

                /*if (patrolPoints != null)//Ѳ�ߵ��б���Ϊ�գ��Ͱ�Ѳ�ߵ��б���ֵ������Ԥ�����Ѳ�ߵ��б�
                {
                    enemy.GetComponent<Enemy>().patrolPoints = patrolPoints;
                }*/

                yield return new WaitForSeconds(enemyData.spawnInterval);
            }
        }
        currentWaveIndex++;
    }

    //�q�ĤH��s�I���H����@��
    private Vector3 GetRandomSpawnPoint()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        return spawnPoints[randomIndex].position;
    }
}

//�S�~��MonoBehaviour�ե�A�Q�n�ǦC�ƭn�K�[[System.Serializable] 
[System.Serializable]
public class EnemyData
{
    public GameObject enemyPrefab;  //�ĤH�w�s��
    public float spawnInterval;     //�ĤH�ͦ����j
    public float waveEnemyCount;    //�ĤH�ƶq
}

[System.Serializable]
public class EnemyWave
{
    public List<EnemyData> enemies; //�C�i�ĤH�C��
}