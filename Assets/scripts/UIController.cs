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
        explvlSlider.maxValue = levelExp;
        explvlSlider.value = currentExp;

        expLvlText.text = "等級: " + currentLvl;
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
