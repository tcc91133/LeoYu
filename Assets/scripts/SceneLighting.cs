using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SceneLighting2D : MonoBehaviour
{
    public Animator buttonAnimator; // 按鈕的 Animator

    public Light2D[] extraFlashlights; // 額外的手電筒光源
    public Light2D globalLight;
    public Light2D flashlightLight; // 手电筒的 Light2D
    public float targetDarkIntensity = 0f;
    public float targetBrightIntensity = 1f;
    public float brighteningDuration = 2f;
    public float darkDelay = 5f; // 離開後多久變暗

    // 平滑變亮
    private bool isBrightening = false;
    private float brighteningTimer = 0f;
    private float startIntensity;

    // 延遲變暗
    private bool isWaitingToDarken = false;
    private float darkTimer = 0.1f;

    private Transform playerTransform; // 玩家位置
    // 音效
    public AudioClip lightOnSound; // 開燈音效
    public AudioClip lightOffSound; // 關燈音效
    private AudioSource audioSource; // 音效來源

    private bool hasPlayedOffSound = false; // 确保关灯音效只播放一次

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
            Debug.LogWarning("No flashlight light assigned. You should assign a flashlight Light2D in the inspector.");
        }

        // 使用标签查找玩家的 Transform
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("No player found with tag 'Player'. Please ensure the player object has the correct tag.");
        }

        // 获取音效组件
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (playerTransform != null)
        {
            flashlightLight.transform.position = playerTransform.position;

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = mousePosition - (Vector2)flashlightLight.transform.position;

            // 算出朝向 + 修正角度
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            flashlightLight.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }

        // 平滑變亮
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

            // 關閉所有手電筒（主 + 額外）
            if (flashlightLight != null)
            {
                flashlightLight.enabled = false;
            }
            foreach (var f in extraFlashlights)
            {
                f.enabled = false;
            }
        }

        // 限制最大亮度為1
        globalLight.intensity = Mathf.Clamp(globalLight.intensity, targetDarkIntensity, targetBrightIntensity);

        // 延遲變暗
        if (isWaitingToDarken)
        {
            darkTimer += Time.deltaTime;
            if (darkTimer >= darkDelay)
            {
                globalLight.intensity = targetDarkIntensity;
                isWaitingToDarken = false;

                // 开启所有手电筒
                if (flashlightLight != null) flashlightLight.enabled = true;
                foreach (var f in extraFlashlights) f.enabled = true;

                // 播放关灯音效
                if (lightOffSound != null && !hasPlayedOffSound)
                {
                    audioSource.PlayOneShot(lightOffSound);
                    hasPlayedOffSound = true; // 确保音效只播放一次
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            if (buttonAnimator != null)
            {
                buttonAnimator.SetBool("IsPressed", true);
            }

            // 播放開燈音效
            if (lightOnSound != null)
            {
                audioSource.PlayOneShot(lightOnSound);
            }

            // 開燈
            StartBrightening();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            if (buttonAnimator != null)
            {
                buttonAnimator.SetBool("IsPressed", false);
            }

            // 延时变暗
            StartDarkening();
        }
    }

    private void StartBrightening()
    {
        if (!isBrightening)
        {
            isBrightening = true;
            startIntensity = globalLight.intensity;
            brighteningTimer = 0f;

            // 关闭所有手电筒光源
            if (flashlightLight != null) flashlightLight.enabled = false;
            foreach (var f in extraFlashlights) f.enabled = false;
        }
    }

    private void StartDarkening()
    {
        if (!isWaitingToDarken)
        {
            isWaitingToDarken = true;
            darkTimer = 0f;
            hasPlayedOffSound = false; // 重置标志，以便在下一次播放音效
        }
    }
}