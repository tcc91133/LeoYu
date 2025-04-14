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

    public float moveSpeed = 5f;
    private Vector2 moveInput;
    private Rigidbody2D playerRigidbody;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    public float pickupRange = 2f;

    public List<Weapon> unassignedWeapons, assignedWeapons;
    public int maxWeapons = 3;

    [HideInInspector]
    public List<Weapon> fullyLevelledWeapons = new List<Weapon>();

    [SerializeField] private List<Transform> mirrorObjects;
    private Vector3[] originalPositions;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

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
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // 根據移動方向翻轉角色
        bool isMovingLeft = moveInput.x < 0;
        bool isMovingRight = moveInput.x > 0;

        if (isMovingLeft || isMovingRight)
        {
            spriteRenderer.flipX = isMovingLeft;
            UpdateMirrorObjects(isMovingLeft);
        }

        anim.SetBool("isMoving", moveInput != Vector2.zero);
    }

    void FixedUpdate()
    {
        playerRigidbody.velocity = moveInput.normalized * moveSpeed;
    }

    private void UpdateMirrorObjects(bool isFacingLeft)
    {
        for (int i = 0; i < mirrorObjects.Count; i++)
        {
            Transform obj = mirrorObjects[i];
            obj.localPosition = isFacingLeft
                ? new Vector3(-originalPositions[i].x, originalPositions[i].y, originalPositions[i].z)
                : originalPositions[i];
        }
    }

    public void AddWeapon(int weaponNumber)
    {
        if (weaponNumber < unassignedWeapons.Count)
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