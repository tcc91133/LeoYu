using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    private Vector2 input;
    private Vector2 mousePos;
    private Animator animator;
    private new Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private List<Transform> mirrorObjects; // 用于存储需要翻转的子物体
    private Vector3[] originalPositions; // 存储原始位置

    private void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // 存储子物体的原始位置
        originalPositions = new Vector3[mirrorObjects.Count];
        for (int i = 0; i < mirrorObjects.Count; i++)
        {
            originalPositions[i] = mirrorObjects[i].localPosition;
        }
    }

    private void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 只翻转玩家的图像
        bool isMouseLeft = mousePos.x < transform.position.x;
        spriteRenderer.flipX = isMouseLeft;

        // 更新镜像物体的位置
        UpdateMirrorObjects(isMouseLeft);

        animator.SetBool("isMoving", input != Vector2.zero);
    }

    private void FixedUpdate()
    {
        rigidbody.velocity = input.normalized * speed;
    }

    private void UpdateMirrorObjects(bool isMouseLeft)
    {
        for (int i = 0; i < mirrorObjects.Count; i++)
        {
            Transform obj = mirrorObjects[i];

            // 计算翻转后的位置
            if (isMouseLeft)
            {
                obj.localPosition = new Vector3(-originalPositions[i].x, originalPositions[i].y, originalPositions[i].z);
            }
            else
            {
                obj.localPosition = originalPositions[i]; // 保持原始位置
            }
        }
    }
}
