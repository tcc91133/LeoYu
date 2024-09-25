using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTracker : MonoBehaviour
{
    [SerializeField] private GameObject missilePrefab;    // 子彈的預製件
    [SerializeField] private Collider2D playerCollider;   // 玩家物件的 Collider
    [SerializeField] private float distanceOffset = 1f;    // 發射點距離
    private bool isFiring = false;  // 用來檢測是否按住左鍵
    private float cooldownTime = 0.45f;  // 冷卻時間
    private float lastFireTime;  // 上次發射的時間

    private Vector3 targetPosition;

    private void Update()
    {
        // 追蹤滑鼠位置
        TrackMousePosition();

        // 當按下左鍵
        if (Input.GetMouseButtonDown(0))
        {
            isFiring = true;  // 開始發射
        }

        // 當釋放左鍵
        if (Input.GetMouseButtonUp(0))
        {
            isFiring = false;  // 停止發射
        }

        // 若按住左鍵且冷卻時間結束，則發射
        if (isFiring && Time.time >= lastFireTime + cooldownTime)
        {
            LaunchMissile();
        }
    }

    private void TrackMousePosition()
    {
        // 將滑鼠螢幕座標轉換為世界座標
        targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0;  // 只關心 2D 平面
    }

    private void LaunchMissile()
    {
        // 取得 Collider 的中心
        Vector3 colliderCenter = playerCollider.bounds.center;

        // 計算方向
        Vector2 direction = (targetPosition - colliderCenter).normalized;

        // 計算發射點位置
        Vector3 firePointPosition = colliderCenter + (Vector3)(direction * distanceOffset);

        // 確保鼠標距離角色的最小距離
        if (Vector2.Distance(colliderCenter, targetPosition) < distanceOffset)
        {
            firePointPosition = colliderCenter + (Vector3)(direction * distanceOffset);
            // 重新計算發射方向以確保不會朝相反方向發射
            direction = (firePointPosition - colliderCenter).normalized;
        }

        // 創建子彈
        GameObject missile = Instantiate(missilePrefab, firePointPosition, Quaternion.identity);

        // 取得子彈的移動腳本，並將目標位置傳遞給它
        MagicMissileMovement missileMovement = missile.GetComponent<MagicMissileMovement>();

        if (missileMovement != null)
        {
            // 設定子彈的移動方向
            Vector2 missileDirection = direction; // 使用計算的方向
            missileMovement.SetDirection(missileDirection);
        }
        else
        {
            Debug.LogError("Missile does not have MagicMissileMovement component!");
        }

        lastFireTime = Time.time; // 更新上次發射的時間
    }
}
