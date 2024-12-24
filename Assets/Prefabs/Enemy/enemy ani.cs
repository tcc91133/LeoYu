using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyani : MonoBehaviour
{
    public Transform player; // 玩家物件
    private Vector3 originalScale;
    private Animator animator;



    private void Start()
    {
        animator = GetComponent<Animator>();

        // 保存敵人的初始縮放值
        originalScale = transform.localScale;
        // 自動尋找帶有 "Player" 標籤的物件
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }


    private void Update()
    {
        if (player != null)
        {
            // 如果玩家在敵人右邊，保持原始方向；在左邊，翻轉 X 軸
            if (player != null)
            {
                // 如果玩家在右側，翻轉 X 軸；在左側，保持原始方向
                if (player.position.x > transform.position.x)
                {
                    transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
                }
                else
                {
                    transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
                }
            }
        }

    }



}
