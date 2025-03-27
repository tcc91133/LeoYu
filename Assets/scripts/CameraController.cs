using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    private Transform target;

    public Vector2 minLimit; // 摄像机可移动的最小 X、Y 值
    public Vector2 maxLimit; // 摄像机可移动的最大 X、Y 值

    void Start()
    {
        target = FindObjectOfType<PlayerController>().transform;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 计算新位置
        float clampedX = Mathf.Clamp(target.position.x, minLimit.x, maxLimit.x);
        float clampedY = Mathf.Clamp(target.position.y, minLimit.y, maxLimit.y);

        // 设定摄像机位置
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
