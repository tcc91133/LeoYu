using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SceneLighting2D : MonoBehaviour
{
    public Animator buttonAnimator;
    public Light2D[] extraFlashlights;
    public Light2D globalLight;
    public Light2D flashlightLight;
    public float targetDarkIntensity = 0f;
    public float targetBrightIntensity = 1f;
    public float brighteningDuration = 2f;

    public Slider batterySlider;
    public float batteryDuration = 30f;

    private bool isBrightening = false;
    private bool isBatteryCounting = false;
    private float brighteningTimer = 0f;
    private float batteryTimer = 0f;
    private float startIntensity;
    private bool hasPlayedOffSound = false;

    private Transform playerTransform;
    private AudioSource audioSource;
    public AudioClip lightOnSound;
    public AudioClip lightOffSound;

    private bool hasTriggered = false; // ✅ 玩家已經觸發過一次
    private bool canTriggerExit = false; // ✅ 防止一開始就觸發退出

    void Start()
    {
        if (globalLight == null)
        {
            globalLight = FindObjectOfType<Light2D>();
            if (globalLight == null)
            {
                Debug.LogError("No Global Light 2D found in the scene!");
            }
        }

        if (flashlightLight == null)
        {
            Debug.LogWarning("No flashlight light assigned.");
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        audioSource = gameObject.AddComponent<AudioSource>();

        // ✅ 開場強制開燈、模擬已經踩過按鈕
        if (buttonAnimator != null)
            buttonAnimator.SetBool("IsPressed", true);

        StartBrightening();

        batteryTimer = batteryDuration;
        batterySlider.value = 1f;
        batterySlider.gameObject.SetActive(true);
        isBatteryCounting = true;

        hasPlayedOffSound = false;
        hasTriggered = true;
        canTriggerExit = true;
    }

    void Update()
    {
        if (playerTransform != null)
        {
            flashlightLight.transform.position = playerTransform.position;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePosition - (Vector2)flashlightLight.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            flashlightLight.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }

        if (isBrightening)
        {
            brighteningTimer += Time.deltaTime;
            float t = brighteningTimer / brighteningDuration;
            if (t >= 1f)
            {
                t = 1f;
                isBrightening = false;
            }

            globalLight.intensity = Mathf.Lerp(startIntensity, targetBrightIntensity, t);
            if (flashlightLight != null) flashlightLight.enabled = false;
            foreach (var f in extraFlashlights) f.enabled = false;
        }

        globalLight.intensity = Mathf.Clamp(globalLight.intensity, targetDarkIntensity, targetBrightIntensity);

        if (isBatteryCounting)
        {
            batteryTimer -= Time.deltaTime;
            if (batteryTimer <= 0f)
            {
                batteryTimer = 0f;
                isBatteryCounting = false;

                ForceDarkening();

                if (lightOffSound != null && !hasPlayedOffSound)
                {
                    audioSource.PlayOneShot(lightOffSound);
                    hasPlayedOffSound = true;
                }
            }

            batterySlider.value = batteryTimer / batteryDuration;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (buttonAnimator != null)
                buttonAnimator.SetBool("IsPressed", true);

            if (lightOnSound != null)
                audioSource.PlayOneShot(lightOnSound);

            batteryTimer = batteryDuration;
            batterySlider.value = 1f;
            batterySlider.gameObject.SetActive(true);
            isBatteryCounting = true;

            hasPlayedOffSound = false;
            hasTriggered = true;

            StartBrightening();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && hasTriggered && canTriggerExit)
        {
            if (buttonAnimator != null)
                buttonAnimator.SetBool("IsPressed", false);

            ForceDarkening();
        }
    }

    private void StartBrightening()
    {
        isBrightening = true;
        startIntensity = globalLight.intensity;
        brighteningTimer = 0f;

        if (flashlightLight != null) flashlightLight.enabled = false;
        foreach (var f in extraFlashlights) f.enabled = false;
    }

    private void ForceDarkening()
    {
        globalLight.intensity = targetDarkIntensity;

        if (flashlightLight != null) flashlightLight.enabled = true;
        foreach (var f in extraFlashlights) f.enabled = true;
    }
}