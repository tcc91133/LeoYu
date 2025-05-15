using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using static CameraController;

public class BossSpawnCleaner : MonoBehaviour
{
    [Header("場景清理設定")]
    public List<GameObject> prefabsToClear = new List<GameObject>();
    public float damageAmount = 9999f;
    public bool shouldKnockBack = false;

    [Header("血條生成設定")]
    public GameObject healthBarPrefab;
    public Transform healthBarParent;
    public string canvasTag = "UICanvas";

    [Header("血條參數")]
    public Vector3 healthBarOffset = new Vector3(0, 100, 0);
    public Color damageFlashColor = new Color(1, 0, 0, 0.3f);
    public float flashDuration = 0.15f;

    private Slider healthSlider;
    private Image damageFlashImage;
    private EnemyController bossController;
    public ShakeType shakeType = ShakeType.Medium;
    private CameraController _cameraController;

    void Start()
    {
        bossController = GetComponent<EnemyController>();
        _cameraController = FindObjectOfType<CameraController>();

        StartCoroutine(CleanScene());
        SpawnBossHealthBar();
        SetupHealthTracking();
    }

    private IEnumerator CleanScene()
    {
        foreach (var obj in FindObjectsOfType<GameObject>())
        {
            if (!obj.scene.IsValid()) continue;

            foreach (var prefab in prefabsToClear)
            {
                if (prefab != null && obj.name.StartsWith(prefab.name))
                {
                    var enemy = obj.GetComponent<EnemyController>();
                    if (enemy != null) enemy.TakeDamage(damageAmount, shouldKnockBack);
                    break;
                }
            }
        }

        _cameraController.ShakeCamera(CameraController.ShakeType.Small);

        // 等一幀，讓敵人有機會生成掉落物
        yield return null;

        foreach (var pickup in GameObject.FindGameObjectsWithTag("PickUps"))
        {
            if (pickup != null)
            {
                Destroy(pickup);
            }
        }
    }



    private void SpawnBossHealthBar()
    {
        if (healthBarPrefab == null) return;

        Transform parent = healthBarParent != null ? healthBarParent :
                         GameObject.FindGameObjectWithTag(canvasTag)?.transform;

        if (parent != null)
        {
            GameObject healthBarObj = Instantiate(healthBarPrefab, parent);
            healthBarObj.transform.localPosition = healthBarOffset;

            healthSlider = healthBarObj.GetComponentInChildren<Slider>();
            damageFlashImage = healthBarObj.GetComponentInChildren<Image>();

            if (healthSlider != null && bossController != null)
            {
                healthSlider.maxValue = bossController.health;
                healthSlider.value = bossController.health;
            }

            if (damageFlashImage != null)
            {
                damageFlashImage.color = new Color(0, 0, 0, 0);
                damageFlashImage.raycastTarget = false;
            }

            GameObject timeTextObj = GameObject.Find("Time Text");
            if (timeTextObj != null)
            {
                timeTextObj.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("生成血條失敗：找不到Canvas");
        }
    }

    private void SetupHealthTracking()
    {
        if (bossController != null && healthSlider != null)
        {
            InvokeRepeating(nameof(UpdateHealth), 0.1f, 0.1f);
        }
    }

    private void UpdateHealth()
    {
        if (bossController == null || healthSlider == null) return;

        float newHealth = bossController.health;

        if (healthSlider.value > newHealth && damageFlashImage != null)
        {
            StartCoroutine(FlashDamage());
        }

        healthSlider.value = newHealth;

        if (newHealth <= 0)
        {
            healthSlider.gameObject.SetActive(false);
            CancelInvoke(nameof(UpdateHealth));
        }
    }

    private IEnumerator FlashDamage()
    {
        damageFlashImage.color = damageFlashColor;
        yield return new WaitForSeconds(flashDuration);

        float fadeTime = 0.2f;
        float elapsed = 0;
        Color startColor = damageFlashColor;
        Color endColor = new Color(0, 0, 0, 0);

        while (elapsed < fadeTime)
        {
            damageFlashImage.color = Color.Lerp(startColor, endColor, elapsed / fadeTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        damageFlashImage.color = endColor;
    }

    void OnDestroy()
    {
        if (healthSlider != null)
        {
            Destroy(healthSlider.gameObject);
        }
    }

    private void CopyRectTransform(GameObject source, GameObject target)
    {
        RectTransform sourceRT = source.GetComponent<RectTransform>();
        RectTransform targetRT = target.GetComponent<RectTransform>();

        if (sourceRT == null || targetRT == null) return;

        targetRT.anchoredPosition = sourceRT.anchoredPosition;
        targetRT.anchorMin = sourceRT.anchorMin;
        targetRT.anchorMax = sourceRT.anchorMax;
        targetRT.pivot = sourceRT.pivot;
        targetRT.sizeDelta = sourceRT.sizeDelta;
        targetRT.localScale = sourceRT.localScale;
        targetRT.localRotation = sourceRT.localRotation;
    }
}
