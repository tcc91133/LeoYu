using UnityEngine;

public class MeleeAttackMovement : MonoBehaviour
{
    [SerializeField] private float radius = 1f;
    [SerializeField] private float attackDuration = 0.5f;
    [SerializeField] private float speed = 1f; // Add speed variable

    private float timer = 0f;
    private Vector2 colliderCenter;

    void Start()
    {
        colliderCenter = GetComponent<Collider2D>().bounds.center;
        transform.position = colliderCenter;
    }

    void Update()
    {
        if (timer < attackDuration)
        {
            timer += Time.deltaTime * speed; // Adjust the timer by speed
            float angle = Mathf.Lerp(0, 180, timer / attackDuration);
            float radian = angle * Mathf.Deg2Rad;

            Vector2 newPosition = colliderCenter + new Vector2(radius * Mathf.Cos(radian), radius * Mathf.Sin(radian));
            transform.position = newPosition;

            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
