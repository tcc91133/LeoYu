using UnityEngine;

public class FirePointAdjuster : MonoBehaviour
{
    [SerializeField] private Transform player;  // 角色的位置
    [SerializeField] private Transform firePoint; // 發射點
    [SerializeField] private float distanceOffset = 1f; // 調整的距離，從 Inspector 調整

    private void Update()
    {
        AdjustFirePoint();
    }

    private void AdjustFirePoint()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // 設置 z 為 0，因為我們只關心 2D 平面

        // 確保角色的 y 軸位置保持不變
        float playerYPosition = player.position.y;

        // 計算發射點的偏移量
        if (mousePosition.x < player.position.x) // 滑鼠在左邊
        {
            firePoint.position = new Vector3(player.position.x - distanceOffset, playerYPosition, firePoint.position.z);
        }
        else // 滑鼠在右邊
        {
            firePoint.position = new Vector3(player.position.x + distanceOffset, playerYPosition, firePoint.position.z);
        }
    }
}
