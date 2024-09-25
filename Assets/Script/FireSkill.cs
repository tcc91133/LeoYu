using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSkill : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int numberOfPrefabs = 5;
    [SerializeField] private float radius = 2f;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float destroyAfterSeconds = 5f;
    [SerializeField] private float skillCooldown = 3f;
    [SerializeField] private Collider2D playerCollider;
    private float lastSkillUseTime;
    private bool canUseSkill = true; 

    private void Update()
    {

        if (canUseSkill && Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseSkill();
            lastSkillUseTime = Time.time;
            canUseSkill = false;
            StartCoroutine(SkillCooldown()); 
        }
    }

    private void UseSkill()
    {
        float angleStep = 360f / numberOfPrefabs;

        for (int i = 0; i < numberOfPrefabs; i++)
        {
            float angle = i * angleStep;
            Vector3 spawnPosition = new Vector3(
                playerCollider.bounds.center.x + Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                playerCollider.bounds.center.y + Mathf.Sin(angle * Mathf.Deg2Rad) * radius,
                0f
            );

            GameObject instance = Instantiate(prefab, spawnPosition, Quaternion.identity);
            instance.AddComponent<RotatingPrefab>().Initialize(playerCollider, radius, angle, rotationSpeed, destroyAfterSeconds);
        }
    }

    private IEnumerator SkillCooldown()
    {
        yield return new WaitForSeconds(skillCooldown);
        canUseSkill = true; 
    }
}

public class RotatingPrefab : MonoBehaviour
{
    private Collider2D playerCollider;
    private float radius;
    private float angle;
    private float rotationSpeed;
    private float lifetime;
    private float timer;

    public void Initialize(Collider2D playerCollider, float radius, float angle, float rotationSpeed, float lifetime)
    {
        this.playerCollider = playerCollider;
        this.radius = radius;
        this.angle = angle;
        this.rotationSpeed = rotationSpeed;
        this.lifetime = lifetime;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        angle += rotationSpeed * Time.deltaTime;

        Vector3 newPosition = new Vector3(
            playerCollider.bounds.center.x + Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
            playerCollider.bounds.center.y + Mathf.Sin(angle * Mathf.Deg2Rad) * radius,
            0f
        );

        transform.position = newPosition;

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Damageable enemy = other.GetComponent<Damageable>();
            if (enemy != null)
            {
                enemy.TakeDamage(10);
                Destroy(gameObject);
            }
        }
    }
}
