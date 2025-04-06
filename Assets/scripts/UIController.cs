using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{

    public static UIController instance;
    private void Awake()
    {
        instance = this;
    }

    public Slider explvlSlider;
    public TMP_Text expLvlText;

    public LevelUpSelectionButton[] levelUpButtons;

    public GameObject levelUpPanel;

    public TMP_Text coinText;

    public PlayerStatUpgradeDisplay moveSpeedUpgradeDisplay, healthUpgradeDisplay, pickupRangeUpgradeDisplay, maxWeaponsUpgradeDispaly;

    public TMP_Text timeText;

    public GameObject levelEndScreen;

    public TMP_Text endTimeText;

    public string mainMenuName;

    public GameObject pauseScreen;
    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            PauseUnpause();

        }
    }

    public void UpdateExperience(int currentExp, int levelExp, int currentLvl)
    {
        // 如果当前等级已经达到30级，直接设置滑块为满值
        if (currentLvl >= 30)
        {
            explvlSlider.maxValue = levelExp;  // 保持最大值为当前的等级经验要求
            explvlSlider.value = levelExp;     // 设置滑块的值为最大值
            expLvlText.text = "已滿級";         // 显示已满级
            levelUpPanel.SetActive(false);     // 隐藏升级UI面板
        }
        else
        {
            explvlSlider.maxValue = levelExp;  // 更新最大值为当前等级所需经验
            explvlSlider.value = currentExp;   // 更新滑块的值为当前经验
            expLvlText.text = "等級: " + currentLvl;  // 显示当前等级
        }
    }

    public void SkipLevelUp() 
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }
    public void UpdateTimer(float time)
    {
        float minutes =Mathf.FloorToInt( time / 60f);
        float seconds = Mathf.FloorToInt(time % 60);

        timeText.text = "時間: " + minutes + ":" + seconds.ToString("00");
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuName);
        Time.timeScale = 1f;
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    public void UpdateCoins() 
    {
        coinText.text = "金錢: " + CoinController.instance.currentCoins;   
    }

    public void PurchaseMoveSpeed()
    {
        PlayerStatController.instance.PurchaseMoveSpeed();
        SFXManager.instance.PlaySFXPitched(12);
    }

    public void PurchaseHealth()
    {
        PlayerStatController.instance.PurchaseHealth();
        SFXManager.instance.PlaySFXPitched(12);
    }

    public void PurchasePickupRange()
    {
        PlayerStatController.instance.PurchasePickupRange();
        SFXManager.instance.PlaySFXPitched(12);
    }

    public void PurchaseMaxWeapon()
    {
        PlayerStatController.instance.PurchaseMaxWeapons();
        SFXManager.instance.PlaySFXPitched(12);
    }

    public void QuitGame()
    { 
        Application.Quit();
    }

    public void PauseUnpause()
    {
        if (pauseScreen.activeSelf == false)
        {
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;
            SFXManager.instance.PlaySFXPitched(3);
        }
        else 
        {
            pauseScreen.SetActive(false);
            if (levelUpPanel.activeSelf == false)
            {
                Time.timeScale = 1f;

                SFXManager.instance.PlaySFXPitched(2);
            }
        }
    }

}
