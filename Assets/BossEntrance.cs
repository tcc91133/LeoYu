using UnityEngine;
using System.Collections;
using Pathfinding;

public class BossEntrance : MonoBehaviour
{
    private CameraController cameraController;
    [Header("动画控制")]
    [SerializeField] private Animator bossAnimator;
    [SerializeField] private string entranceAnimationName = "Entrance";
    [SerializeField] private float animationSpeedMultiplier = 1f;

    [Header("相机设置")]
    [SerializeField] private float zoomSize = 5f; // 新增：相机放大尺寸
    [SerializeField] private float zoomDuration = 0.5f; // 新增：放大过渡时间

    [Header("目标设置")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private SceneLighting2D sceneLighting;

    private Transform playerTransform;
    private bool isEntranceComplete = false;
    private AIPath aiPath;
    private Camera mainCamera;
    private float originalCameraSize;

    private void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            originalCameraSize = mainCamera.orthographicSize;
        }

        sceneLighting = FindObjectOfType<SceneLighting2D>();
        if (sceneLighting == null)
        {
            Debug.LogError("找不到 SceneLighting2D！請確認場景中有該組件。");
        }

        if (bossAnimator == null)
            bossAnimator = GetComponent<Animator>();

        aiPath = GetComponent<AIPath>();

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

        cameraController = FindObjectOfType<CameraController>();
        if (cameraController == null)
        {
            Debug.LogError("找不到 CameraController，請確認場景中有該元件");
        }
    }

    private void Start()
    {
        if (!enabled) return;

        if (aiPath != null)
            aiPath.enabled = false;

        StartCoroutine(EntranceSequence());
    }

    private IEnumerator EntranceSequence()
    {
        Debug.Log("开始Boss出场动画");

        // 重置灯光
        if (sceneLighting != null)
        {
            sceneLighting.ResetBattery();
            sceneLighting.StartBrightening();
        }

        // 切换相机目标并放大
        if (cameraController != null)
        {
            cameraController.SetTarget(transform);
            yield return StartCoroutine(ZoomCamera(zoomSize, zoomDuration)); // 平滑放大
        }

        // 播放动画
        if (bossAnimator != null)
        {
            bossAnimator.Play(entranceAnimationName, -1, 0f);
            bossAnimator.speed = animationSpeedMultiplier;
            bossAnimator.Update(0f);
        }

        float animLength = GetAnimationLength(entranceAnimationName) / animationSpeedMultiplier;
        yield return new WaitForSeconds(animLength);

        OnEntranceComplete();

        // 切换回玩家并恢复相机
        if (cameraController != null)
        {
            yield return StartCoroutine(ZoomCamera(originalCameraSize, zoomDuration)); // 平滑恢复
            cameraController.ResetToPlayer();
        }
    }

    // 新增：平滑缩放相机协程
    private IEnumerator ZoomCamera(float targetSize, float duration)
    {
        if (mainCamera == null) yield break;

        float initialSize = mainCamera.orthographicSize;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            mainCamera.orthographicSize = Mathf.Lerp(initialSize, targetSize, time / duration);
            yield return null;
        }

        mainCamera.orthographicSize = targetSize;
    }

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

    private void OnEntranceComplete()
    {
        Debug.Log("Boss出场动画完成");

        if (bossAnimator != null)
        {
            bossAnimator.speed = 1f;
            bossAnimator.Play("Idle");
        }

        if (aiPath != null)
            aiPath.enabled = true;

        isEntranceComplete = true;
    }

    public bool IsEntranceComplete() => isEntranceComplete;
}