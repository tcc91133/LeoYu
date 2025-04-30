using UnityEngine;
using System.Collections; // 添加這行以使用 IEnumerator

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashDistanceMultiplier = 2f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    private Vector2 _lastDirection;
    private bool _canDash = true;
    private bool _isDashing;
    private PlayerController playerController;
    private Coroutine _dashCoroutine;

    private void Start()
    {
        playerController = GetComponent<PlayerController>() ?? GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        _lastDirection = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        if (Input.GetKeyDown(KeyCode.Space) && _canDash && _lastDirection != Vector2.zero && !_isDashing)
        {
            SFXManager.instance.PlaySFXPitched(15);
            _dashCoroutine = StartCoroutine(DashCoroutine(_lastDirection));
        }
    }

    private IEnumerator DashCoroutine(Vector2 direction)
    {
        _canDash = false;
        _isDashing = true;

        float currentMoveSpeed = playerController.moveSpeed;
        float dashDistance = currentMoveSpeed * dashDistanceMultiplier;
        Vector2 startPosition = transform.position;
        Vector2 targetPosition = startPosition + direction * dashDistance;

        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / dashDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        yield return new WaitForSeconds(dashCooldown);

        _isDashing = false;
        _canDash = true;
    }

    public void OnDeath()
    {
        if (_dashCoroutine != null)
        {
            StopCoroutine(_dashCoroutine);
            _isDashing = false;
            _canDash = true;
        }
    }

    private void OnDisable()
    {
        OnDeath();
    }
}