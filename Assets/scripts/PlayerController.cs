using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour

{
    public static PlayerController instance;
    private void Awake()
    {
        instance = this;
    }
    public float moveSpeed = 5f; // 移动速度
    private Vector2 moveInput; // 移动
    private Vector3 mousePos; // 鼠标位置
    private Rigidbody2D playerRigidbody;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    public float pickupRange = 2f;
    //public Weapon activeWeapon;

    public List<Weapon> unassignedWeapons, assignedWeapons;

    public int maxWeapons = 3;

    [HideInInspector]
    public List<Weapon> fullyLevelledWeapons = new List<Weapon>();

   [SerializeField] private List<Transform> mirrorObjects; // 存储需要翻转的子物体
    private Vector3[] originalPositions; // 存储原始位置

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // 存储子物体的原始位置
        originalPositions = new Vector3[mirrorObjects.Count];
        for (int i = 0; i < mirrorObjects.Count; i++)
        {
            originalPositions[i] = mirrorObjects[i].localPosition;
        }

        if (assignedWeapons.Count == 0) 
        {
            AddWeapon(Random.Range(0, unassignedWeapons.Count));
        }

        moveSpeed = PlayerStatController.instance.moveSpeed[0].value;
        pickupRange = PlayerStatController.instance.pickupRange[0].value;
        maxWeapons = Mathf.RoundToInt(PlayerStatController.instance.maxWeapons[0].value);
    }

    void Update()
    {
        // 获取移动输入
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 只翻转角色的图片
        bool isMouseLeft = mousePos.x < transform.position.x;
        spriteRenderer.flipX = isMouseLeft;

        // 更新镜像物体的位置
        UpdateMirrorObjects(isMouseLeft);

        // 更新动画
        anim.SetBool("isMoving", moveInput != Vector2.zero);
    }

    void FixedUpdate()
    {
        // 更新角色速度
        playerRigidbody.velocity = moveInput.normalized * moveSpeed;
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
    public void AddWeapon(int weaponNumber)
    {
        if (weaponNumber<unassignedWeapons.Count)
        {
            assignedWeapons.Add(unassignedWeapons[weaponNumber]);

            unassignedWeapons[weaponNumber].gameObject.SetActive(true);
            unassignedWeapons.RemoveAt(weaponNumber);
        
        }
    
    }
    public void AddWeapon(Weapon weaponToAdd) 
    {
        weaponToAdd.gameObject.SetActive(true);

        assignedWeapons.Add(weaponToAdd);
        unassignedWeapons.Remove(weaponToAdd);
    }
}
