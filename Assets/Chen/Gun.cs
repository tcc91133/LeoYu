using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    protected Vector2 mousePos;       // 鼠标位置
    protected Camera mainCamera;      // 主摄像机引用
    private Vector3 originalScale;    // 保存原始缩放比例

    protected virtual void Start()
    {
        // 缓存主摄像机以提高效率
        mainCamera = Camera.main;
        originalScale = transform.localScale; // 保存原始缩放比例
    }

    protected virtual void Update()
    {
        // 获取鼠标在世界空间的位置
        mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // 计算从枪到鼠标的方向
        Vector2 direction = mousePos - (Vector2)transform.position;

        // 计算旋转角度
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 应用旋转
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 保持枪的缩放不变
        transform.localScale = originalScale;

        // 根据鼠标位置翻转枪的缩放
        if (mousePos.x < transform.position.x)
        {
            transform.localScale = new Vector3(originalScale.x, -originalScale.y, originalScale.z); // 翻转
        }
        else
        {
            transform.localScale = originalScale; // 重置翻转
        }
    }
}
