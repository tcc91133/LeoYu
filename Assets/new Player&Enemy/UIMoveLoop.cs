using UnityEngine;

public class MoveLoop : MonoBehaviour
{
    public float moveDistance = 25f;  // 移动距离
    public float speed = 5f;         // 速度
    private Vector3 startPos;
    private Vector3 endPos;
    private bool movingForward = true;
    private SpriteRenderer spriteRenderer; // 角色的 SpriteRenderer

    void Start()
    {
        startPos = transform.position;
        endPos = startPos + new Vector3(-moveDistance, 0, 0);
        spriteRenderer = GetComponent<SpriteRenderer>(); // 获取 SpriteRenderer
    }

    void Update()
    {
        if (movingForward)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPos, Time.deltaTime * speed);
            spriteRenderer.flipX = false; // 正常朝向
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, Time.deltaTime * speed);
            spriteRenderer.flipX = true; // 翻转朝向
        }

        if (Vector3.Distance(transform.position, endPos) < 0.1f) movingForward = false;
        if (Vector3.Distance(transform.position, startPos) < 0.1f) movingForward = true;
    }
}