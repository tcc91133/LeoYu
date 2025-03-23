using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseAttackWeapon : Weapon
{
    public EnemyDamager damager;

    private Transform playerTransform;
    private float attackCounter;

    void Start()
    {
        SetStats();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (statsUpdated)
        {
            statsUpdated = false;
            SetStats();
        }

        attackCounter -= Time.deltaTime;
        if (attackCounter <= 0)
        {
            attackCounter = stats[weaponLevel].timeBetweenAttacks;

            // 计算鼠标方向
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;

            Vector3 directionToMouse = (mousePosition - playerTransform.position).normalized;
            float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

            Quaternion baseRotation = Quaternion.Euler(0f, 0f, angle);

            // 生成主攻击
            Instantiate(damager, damager.transform.position, baseRotation, transform).gameObject.SetActive(true);

            // 生成额外攻击
            for (int i = 1; i < stats[weaponLevel].amount; i++)
            {
                float rotAngle = (360f / stats[weaponLevel].amount) * i;
                Quaternion extraRotation = Quaternion.AngleAxis(rotAngle, Vector3.forward) * baseRotation;

                Instantiate(damager, damager.transform.position, extraRotation, transform).gameObject.SetActive(true);
            }

            SFXManager.instance.PlaySFXPitched(10);
        }
    }

    void SetStats()
    {
        damager.damageAmount = stats[weaponLevel].damage;
        damager.lifeTime = stats[weaponLevel].duration;
        damager.transform.localScale = Vector3.one * stats[weaponLevel].range;
        attackCounter = 0f;
    }
}