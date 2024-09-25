using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCollider : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) 
        {
            Damageable enemy = collision.GetComponent<Damageable>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageAmount);
            }
            Destroy(gameObject); 
        }
    }
}