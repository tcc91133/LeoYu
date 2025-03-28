using System.Collections;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashDistanceMultiplier = 2f; // 冲刺距离倍数
    [SerializeField] private float dashDuration = 0.2f; // 冲刺持续时间
    [SerializeField] private float dashCooldown = 1f; // 冷却时间

    private Vector2 _lastDirection;
    private bool _canDash = true;
    private PlayerController playerController;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            playerController = GetComponentInParent<PlayerController>();
        }
    }

    private void Update()
    {
        // 获取输入方向
        _lastDirection = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        // 检测冲刺输入
        if (Input.GetKeyDown(KeyCode.Space) && _canDash && _lastDirection != Vector2.zero)
        {
            SFXManager.instance.PlaySFXPitched(15);
            StartCoroutine(DashCoroutine(_lastDirection));
        }
    }

    private IEnumerator DashCoroutine(Vector2 direction)
    {
        _canDash = false;

        // 计算冲刺参数
        float currentMoveSpeed = playerController.moveSpeed;
        float dashDistance = currentMoveSpeed * dashDistanceMultiplier;
        float dashSpeed = dashDistance / dashDuration;

        Vector2 startPosition = transform.position;
        Vector2 targetPosition = startPosition + direction * dashDistance;

        // 执行冲刺
        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            float t = elapsedTime / dashDuration;
            transform.position = Vector2.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保最终位置准确
        transform.position = targetPosition;

        // 冷却时间
        yield return new WaitForSeconds(dashCooldown);
        _canDash = true;
    }
}