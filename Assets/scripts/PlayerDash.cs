using System.Collections;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashSpeed = 10f;
    private Vector2 _lastDirection;
    private bool _canDash = true;

    private void Update()
    {
        _lastDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (Input.GetKeyDown(KeyCode.Space) && _canDash)
        {
            if (_lastDirection != Vector2.zero)
            {
                StartCoroutine(DashCoroutine(_lastDirection));
            }
        }
    }

    private IEnumerator DashCoroutine(Vector2 direction)
    {
        _canDash = false;
        Vector2 startPosition = transform.position;
        Vector2 targetPosition = startPosition + direction * dashDistance;

        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / dashDuration);
            elapsedTime += Time.deltaTime * dashSpeed;
            yield return null;
        }
        transform.position = targetPosition;

        yield return new WaitForSeconds(dashCooldown);
        _canDash = true;
    }
}
