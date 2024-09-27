using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PlayerDash : MonoBehaviour
{
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
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

    private void MoveCharacter(Vector2 direction)
    {
        var targetPosition = (Vector2)transform.position + direction * dashDistance;
        transform.DOMove(targetPosition, dashDuration).SetEase(Ease.OutCubic);
    }

    private IEnumerator DashCoroutine(Vector2 direction)
    {
        _canDash = false;
        MoveCharacter(direction);
        yield return new WaitForSeconds(dashDuration);
        yield return new WaitForSeconds(dashCooldown);
        _canDash = true;
    }
}
