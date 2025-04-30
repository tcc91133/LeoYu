using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Collections;

public class SceneLighting2D : MonoBehaviour
{
    [Header("Light References")]
    public Animator buttonAnimator;
    public Light2D[] extraFlashlights;
    public Light2D globalLight;
    public Light2D flashlightLight;

    [Header("Light Settings")]
    public float targetDarkIntensity = 0f;
    public float targetBrightIntensity = 1f;
    public float brighteningDuration = 2f;

    [Header("Battery Settings")]
    public Slider batterySlider;
    public float batteryDuration = 30f;
    public Color normalBatteryColor = Color.yellow;
    public Color flashColor1 = Color.red;
    public Color flashColor2 = new Color(1f, 0.5f, 0f);

    [Header("Audio")]
    public AudioClip lightOnSound;
    public AudioClip lightOffSound;
    public AudioClip castModeSound; // 新增：Cast模式专用音效

    // 状态变量
    private bool isBrightening = false;
    private bool isBatteryCounting = false;
    private float brighteningTimer = 0f;
    private float batteryTimer = 0f;
    private float startIntensity;
    private bool hasPlayedOffSound = false;
    private bool isLocked = false;
    private bool isFlashing = false;
    private bool isInCastMode = false; // 新增：Cast模式标志
    private Transform playerTransform;
    private AudioSource audioSource;
    private Image batteryFillImage;
    private Coroutine flashCoroutine;

    // 属性封装
    public float BatteryTimer
    {
        get => batteryTimer;
        set => batteryTimer = Mathf.Clamp(value, 0f, batteryDuration);
    }

    public bool IsInCastMode
    {
        get => isInCastMode;
        set => isInCastMode = value;
    }

    void Start()
    {
        InitializeComponents();
        SetupInitialState();
    }

    void InitializeComponents()
    {
        if (globalLight == null)
            globalLight = FindObjectOfType<Light2D>();

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

        audioSource = gameObject.AddComponent<AudioSource>();

        if (batterySlider != null)
            batteryFillImage = batterySlider.fillRect.GetComponent<Image>();
    }

    void SetupInitialState()
    {
        if (buttonAnimator != null)
            buttonAnimator.SetBool("IsPressed", true);

        StartBrightening();

        BatteryTimer = batteryDuration;
        UpdateBatteryUI();
        isBatteryCounting = true;
        hasPlayedOffSound = false;
    }

    void Update()
    {
        UpdateFlashlightPosition();
        UpdateLighting();
        UpdateBattery();
    }

    void UpdateFlashlightPosition()
    {
        if (playerTransform == null || flashlightLight == null) return;

        flashlightLight.transform.position = playerTransform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - (Vector2)flashlightLight.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        flashlightLight.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    void UpdateLighting()
    {
        if (isBrightening)
        {
            brighteningTimer += Time.deltaTime;
            float t = Mathf.Clamp01(brighteningTimer / brighteningDuration);

            globalLight.intensity = Mathf.Lerp(startIntensity, targetBrightIntensity, t);
            SetFlashlightsEnabled(false);

            if (t >= 1f)
                isBrightening = false;
        }

        globalLight.intensity = Mathf.Clamp(globalLight.intensity, targetDarkIntensity, targetBrightIntensity);
    }

    void UpdateBattery()
    {
        if (!isBatteryCounting || isLocked) return;

        BatteryTimer -= Time.deltaTime;
        UpdateBatteryUI();

        if (BatteryTimer <= 0f)
        {
            BatteryTimer = 0f;
            isBatteryCounting = false;
            ForceDarkening();

            if (lightOffSound != null && !hasPlayedOffSound)
            {
                audioSource.PlayOneShot(lightOffSound);
                hasPlayedOffSound = true;
            }
        }
    }

    void UpdateBatteryUI()
    {
        if (batterySlider != null)
            batterySlider.value = BatteryTimer / batteryDuration;
    }

    void SetFlashlightsEnabled(bool enabled)
    {
        if (flashlightLight != null)
            flashlightLight.enabled = enabled;

        foreach (var f in extraFlashlights)
            f.enabled = enabled;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // 始终播放按钮动画
        if (buttonAnimator != null)
            buttonAnimator.SetBool("IsPressed", true);

        // 根据模式播放不同音效
        if (isInCastMode && castModeSound != null)
        {
            audioSource.PlayOneShot(castModeSound);
        }
        else if (!isLocked && lightOnSound != null)
        {
            audioSource.PlayOneShot(lightOnSound);
        }

        // 只有在非Cast模式且未锁定时才实际生效
        if (!isInCastMode && !isLocked)
        {
            ResetBattery();
            StartBrightening();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (buttonAnimator != null)
            buttonAnimator.SetBool("IsPressed", false);

        // 只有在非Cast模式且电池计数中时才强制关灯
        if (!isInCastMode && isBatteryCounting)
            ForceDarkening();
    }

    public void ResetBattery()
    {
        BatteryTimer = batteryDuration;
        isBatteryCounting = true;
        hasPlayedOffSound = false;
        UpdateBatteryUI();
    }

    public void StartBrightening()
    {
        isBrightening = true;
        startIntensity = globalLight.intensity;
        brighteningTimer = 0f;
        SetFlashlightsEnabled(false);
    }

    public void ForceDarkening()
    {
        globalLight.intensity = targetDarkIntensity;
        SetFlashlightsEnabled(true);
    }

    public void ForceCompleteDarken()
    {
        globalLight.intensity = targetDarkIntensity;
        BatteryTimer = 0f;
        isBatteryCounting = false;
        UpdateBatteryUI();
        SetFlashlightsEnabled(true);

        if (lightOffSound != null && !hasPlayedOffSound)
        {
            audioSource.PlayOneShot(lightOffSound);
            hasPlayedOffSound = true;
        }
    }

    public void LockControls(bool shouldFlash)
    {
        isLocked = true;
        isBatteryCounting = false;

        if (shouldFlash && batteryFillImage != null)
        {
            isFlashing = true;
            if (flashCoroutine != null)
                StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashBatterySlider());
        }
    }

    public void UnlockControls()
    {
        isLocked = false;
        isFlashing = false;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        if (batteryFillImage != null)
            batteryFillImage.color = normalBatteryColor;
    }

    private IEnumerator FlashBatterySlider()
    {
        while (isFlashing)
        {
            batteryFillImage.color = flashColor1;
            yield return new WaitForSeconds(0.3f);
            batteryFillImage.color = flashColor2;
            yield return new WaitForSeconds(0.3f);
        }

        batteryFillImage.color = normalBatteryColor;
    }
}