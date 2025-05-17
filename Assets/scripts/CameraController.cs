using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform target;
    public Vector2 minLimit;
    public Vector2 maxLimit;

    private bool isShaking = false;
    private Vector3 shakeOffset = Vector3.zero;

    [System.Serializable]
    public class ShakeSettings
    {
        public float duration;
        public float magnitude;
        public AnimationCurve decayCurve = AnimationCurve.Linear(0, 1, 1, 0);
    }

    public enum ShakeType { Small, Medium, Large }

    public ShakeSettings smallShake;
    public ShakeSettings mediumShake;
    public ShakeSettings largeShake;

    void Start()
    {
        target = FindObjectOfType<PlayerController>().transform;
    }

    void LateUpdate()
    {
        if (target == null) return;

        float clampedX = Mathf.Clamp(target.position.x, minLimit.x, maxLimit.x);
        float clampedY = Mathf.Clamp(target.position.y, minLimit.y, maxLimit.y);
        Vector3 targetPosition = new Vector3(clampedX, clampedY, transform.position.z);

        transform.position = targetPosition + shakeOffset;
    }

    // ✅ 新增：不給參數，預設用 Small
    public void ShakeCamera()
    {
        ShakeCamera(ShakeType.Small);
    }

    public void ShakeCamera(ShakeType type)
    {
        if (!isShaking)
        {
            ShakeSettings settings = GetShakeSettings(type);
            StartCoroutine(Shake(settings.duration, settings.magnitude, settings.decayCurve));
        }
    }

    private ShakeSettings GetShakeSettings(ShakeType type)
    {
        switch (type)
        {
            case ShakeType.Small: return smallShake;
            case ShakeType.Medium: return mediumShake;
            case ShakeType.Large: return largeShake;
            default: return smallShake;
        }
    }

    private IEnumerator Shake(float duration, float magnitude, AnimationCurve decayCurve)
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float progress = elapsed / duration;
            float decayFactor = decayCurve.Evaluate(progress);
            float currentMagnitude = magnitude * decayFactor;

            shakeOffset = (Vector3)Random.insideUnitCircle * currentMagnitude;

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
        isShaking = false;
    }
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // ✅ 新增：回到預設玩家
    public void ResetToPlayer()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            target = player.transform;
        }
    }
}
