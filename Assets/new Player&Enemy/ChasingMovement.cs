using UnityEngine;

public class ChasingWithDistance : MonoBehaviour
{
    public Transform target;  // 目标物体
    public float speed = 5f;  // 追逐速度
    public float minDistance = 3f; // 保持的最小距离

    private SpriteRenderer spriteRenderer; // 角色的 SpriteRenderer

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // 获取 SpriteRenderer
    }

    void Update()
    {
        if (target != null)
        {
            // 计算目标与当前物体之间的方向，但只关注x轴
            Vector3 direction = new Vector3(target.position.x - transform.position.x, 0, 0);
            float distance = direction.magnitude;  // 计算当前x轴上的距离

            // 如果距离大于最小距离，则继续追逐
            if (distance > minDistance)
            {
                direction.Normalize(); // 将方向单位化
                // 只修改x轴的坐标，y轴保持不变
                transform.position += direction * speed * Time.deltaTime;
            }

            // 控制角色的朝向，只有x轴方向来判断
            if (target.position.x > transform.position.x)
                spriteRenderer.flipX = true; // 目标在右侧，角色面朝左
            else
                spriteRenderer.flipX = false;  // 目标在左侧，角色面朝右
        }
    }
}