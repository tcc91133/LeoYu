using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    public void CheckDeath(int health)
    {
        if (health <= 0)
            Die();
    }

    private void Die()
    {
        Destroy(gameObject); 
    }
}
