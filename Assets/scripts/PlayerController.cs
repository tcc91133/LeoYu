using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; //���ʳt��
    public Animator anim; 
    private Rigidbody2D playerRigidbody; 
    private Vector3 mousePos; // �ƹ�
    private Vector3 moveInput; // ����

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
        // �������
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        // ��s�ʵe
        anim.SetBool("isMoving", moveInput != Vector3.zero);

        // ����ݼ�
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // �]�w�}�⭱�V
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
        // ��s�}��t��
        playerRigidbody.velocity = moveInput * moveSpeed;
    }

}