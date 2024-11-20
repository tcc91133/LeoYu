using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; //移動速度
    public Animator anim; 
    private Rigidbody2D playerRigidbody; 
    private Vector3 mousePos; // 滑鼠
    private Vector3 moveInput; // 移動

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
    }

    void Update()
    {
        // 獲取移動
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        // 更新動畫
        anim.SetBool("isMoving", moveInput != Vector3.zero);

        // 獲取屬標
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 設定腳色面向
        if (mousePos.x > transform.position.x)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
    }

    void FixedUpdate()
    {
        // 更新腳色速度
        playerRigidbody.velocity = moveInput * moveSpeed;
    }

}
