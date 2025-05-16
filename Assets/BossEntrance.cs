using UnityEngine;
using System.Collections;
using Pathfinding;

public class BossEntrance : MonoBehaviour
{
    [Header("动画控制")]
    [SerializeField] private Animator bossAnimator;             // Boss动画控制器
    [SerializeField] private string entranceAnimationName = "Entrance"; // 出场动画名称
    [SerializeField] private float animationSpeedMultiplier = 1f; // 动画速度 multiplier

    [Header("目标设置")]
    [SerializeField] private string playerTag = "Player";       // 玩家标签

    private Transform playerTransform;
    private bool isEntranceComplete = false;
    private AIPath aiPath;

    private void Awake()
    {
        // 获取动画组件
        if (bossAnimator == null)
            bossAnimator = GetComponent<Animator>();

        // 获取AI路径组件
        aiPath = GetComponent<AIPath>();

        // 查找玩家
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogError($"找不到带有 {playerTag} 标签的玩家对象！", this);
            enabled = false;
        }
    }

    private void Start()
    {
        if (!enabled) return;

        // 禁用AI移动
        if (aiPath != null)
            aiPath.enabled = false;

        // 开始出场流程
        StartCoroutine(EntranceSequence());
    }

    private IEnumerator EntranceSequence()
    {
        Debug.Log("开始Boss出场动画");

        // 1. 播放出场动画
        if (bossAnimator != null)
        {
            bossAnimator.Play(entranceAnimationName, -1, 0f);
            bossAnimator.speed = animationSpeedMultiplier;

            // 强制立即更新动画状态
            bossAnimator.Update(0f);
        }

        // 2. 等待动画完成
        float animLength = GetAnimationLength(entranceAnimationName) / animationSpeedMultiplier;
        yield return new WaitForSeconds(animLength);

        // 3. 完成出场
        OnEntranceComplete();
    }

    /// <summary>
    /// 获取动画长度
    /// </summary>
    private float GetAnimationLength(string animationName)
    {
        if (bossAnimator == null || bossAnimator.runtimeAnimatorController == null)
            return 0f;

        foreach (AnimationClip clip in bossAnimator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == animationName)
                return clip.length;
        }

        Debug.LogError($"找不到名为 {animationName} 的动画片段", this);
        return 0f;
    }

    /// <summary>
    /// 出场完成处理
    /// </summary>
    private void OnEntranceComplete()
    {
        Debug.Log("Boss出场动画完成");

        // 重置动画状态
        if (bossAnimator != null)
        {
            bossAnimator.speed = 1f;
            bossAnimator.Play("Idle");
        }

        // 重新启用AI移动
        if (aiPath != null)
            aiPath.enabled = true;

        isEntranceComplete = true;
    }

    public bool IsEntranceComplete() => isEntranceComplete;
}