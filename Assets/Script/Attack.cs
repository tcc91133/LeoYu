using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Timers;
using UnityEngine.Events;

public class Attack : MonoBehaviour
{
    [SerializeField] private string targetTag;            // 目标标签
    [SerializeField] private UnityEvent attack;            // 攻击事件
    [SerializeField] private float damageCooldown = 1f;    // 每个目标的伤害冷却时间
    [SerializeField] private int damageAmount = 1;         // 攻击造成的伤害值，可以在 Inspector 中调整

    private Dictionary<Collider2D, float> lastDamageTime = new Dictionary<Collider2D, float>(); // 记录每个目标的最后伤害时间

    private void OnTriggerEnter2D(Collider2D col)
    {
        DealDamage(col);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        DealDamage(other);
    }

    private void DealDamage(Collider2D other)
    {
        if (!other.CompareTag(targetTag)) return;

        if (!lastDamageTime.TryGetValue(other, out float lastAttackTime) || Time.time >= lastAttackTime + damageCooldown)
        {
            var damageable = other.GetComponent<Damageable>();
            damageable.TakeDamage(damageAmount); // 使用 damageAmount 造成伤害
            lastDamageTime[other] = Time.time; // 更新目标的最后伤害时间
            attack.Invoke();
        }
    }
}
