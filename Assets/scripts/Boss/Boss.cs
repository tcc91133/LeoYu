using System.Collections.Generic;
using UnityEngine;

public class BossSpawnCleaner : MonoBehaviour
{
    public List<GameObject> prefabsToClear = new List<GameObject>();
    public float damageAmount = 9999f;
    public bool shouldKnockBack = false;

    void Start()
    {
        var objectsInScene = FindObjectsOfType<GameObject>();
        foreach (var obj in objectsInScene)
        {
            if (!obj.scene.IsValid()) continue;

            foreach (var prefab in prefabsToClear)
            {
                if (prefab == null) continue;

                if (obj.name.StartsWith(prefab.name))
                {
                    EnemyController enemy = obj.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damageAmount, shouldKnockBack);
                    }
                    break;
                }
            }
        }
    }
}
