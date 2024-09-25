using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissileMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;

    private Vector2 _direction;

    public void SetDirection(Vector2 direction)
    {
        _direction = direction;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, _direction);
    }

    private void FixedUpdate()
    {
        if (_direction != Vector2.zero)
        {
            var targetPosition = (Vector2)transform.position + _direction;
            rb.DOMove(targetPosition, speed).SetSpeedBased();
        }
    }
}
