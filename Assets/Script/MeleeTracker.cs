using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeTracker : MonoBehaviour
{
    [SerializeField] private GameObject meleePrefab; // 近战攻击的预制件
    [SerializeField] private float distanceOffset = 1f; // 发射点距离
    [SerializeField] private float cooldownTime = 0.45f; // 冷却时间
    [SerializeField] private Transform meleeFollowPoint; // 跟随的中心点
    private bool isFiring = false; // 用来检测是否按住右键
    private float lastAttackTime; // 上次攻击的时间

    private GameObject currentMeleeEffect; // 记录当前生成的近战攻击效果
    private Vector3 targetPosition;
    private Vector3 attackDirection; // 攻击方向

    private void Start()
    {
        lastAttackTime = Time.time - cooldownTime; // 初始化上次攻击时间，使开局冷却有效
    }

    private void Update()
    {
        // 追踪鼠标位置
        TrackMousePosition();

        // 当按下右键
        if (Input.GetMouseButtonDown(1))
        {
            isFiring = true; // 开始攻击
        }

        // 当释放右键
        if (Input.GetMouseButtonUp(1))
        {
            isFiring = false; // 停止攻击
        }

        // 若按住右键且冷却时间结束，则发起攻击
        if (isFiring && Time.time >= lastAttackTime + cooldownTime)
        {
            PerformMeleeAttack();
        }

        // 更新生成的近战攻击效果的位置和角度
        if (currentMeleeEffect != null)
        {
            UpdateMeleeEffectPositionAndRotation();
        }
    }

    private void TrackMousePosition()
    {
        // 将鼠标屏幕坐标转换为世界坐标
        targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0; // 只关心 2D 平面
    }

    private void PerformMeleeAttack()
    {
        // 如果当前已经有近战攻击效果，直接返回
        if (currentMeleeEffect != null) return;

        // 计算攻击方向
        attackDirection = (targetPosition - transform.position).normalized;

        // 创建近战攻击效果
        currentMeleeEffect = Instantiate(meleePrefab, meleeFollowPoint.position, Quaternion.identity); // 在 meleeFollowPoint 位置生成

        // 设置初始位置
        Vector3 attackPosition = meleeFollowPoint.position + attackDirection * distanceOffset;
        currentMeleeEffect.transform.position = attackPosition;

        // 使物体的 Z 轴指向鼠标
        currentMeleeEffect.transform.up = attackDirection; // Z轴朝向鼠标方向

        lastAttackTime = Time.time; // 更新上次攻击的时间
    }

    private void UpdateMeleeEffectPositionAndRotation()
    {
        // 更新近战攻击效果的位置
        Vector3 attackPosition = meleeFollowPoint.position + attackDirection * distanceOffset;
        currentMeleeEffect.transform.position = attackPosition;

        // 保持物体的角度不变
        currentMeleeEffect.transform.up = attackDirection; // Z轴朝向原来的攻击方向
    }
}
