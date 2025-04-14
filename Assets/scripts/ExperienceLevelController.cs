using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceLevelController : MonoBehaviour
{
    public static ExperienceLevelController instance;

    void Awake()
    {
        instance = this;
    }

    public int currentExperience;

    public ExpPickup pickup;

    public List<int> expLevels;
    public int currentLevel = 1, levelCount = 100;
    public int maxExpPerLevel = 35; // 新增：每級最大所需經驗值

    public List<Weapon> weaponsToUpgrade;

    void Start()
    {
        expLevels.Clear();
        expLevels.Add(3); // 等級1 → 2

        for (int i = 1; i < levelCount; i++)
        {
            int nextExp = Mathf.CeilToInt(expLevels[i - 1] * 1.2f); // 每級都 *2
            expLevels.Add(nextExp);
        }
    }

    public void GetExp(int amountToGet)
    {
        currentExperience += amountToGet;

        if (currentExperience >= expLevels[currentLevel])
        {
            LevelUp();
        }

        UIController.instance.UpdateExperience(currentExperience, expLevels[currentLevel], currentLevel);
    }

    public void SpawnExp(Vector3 position, int expValue)
    {
        Instantiate(pickup, position, Quaternion.identity).expValue = expValue;
    }

    void LevelUp()
    {
        currentExperience -= expLevels[currentLevel];

        currentLevel++;

        if (currentLevel >= expLevels.Count)
        {
            currentLevel = expLevels.Count - 1;
        }

        // 当等级达到30级时，不再显示升级UI
        if (currentLevel >= 20)
        {
            UIController.instance.levelUpPanel.SetActive(false);  // 隐藏升级UI面板
            return;  // 不继续执行后续的升级操作
        }

        UIController.instance.levelUpPanel.SetActive(true);  // 显示升级UI面板

        Time.timeScale = 0f;


        weaponsToUpgrade.Clear();

        List<Weapon> availableWeapons = new List<Weapon>();

        availableWeapons.AddRange(PlayerController.instance.assignedWeapons);

        if (availableWeapons.Count > 0)
        {
            int selected = Random.Range(0, availableWeapons.Count);
            weaponsToUpgrade.Add(availableWeapons[selected]);
            availableWeapons.RemoveAt(selected);
        }

        if (PlayerController.instance.assignedWeapons.Count + PlayerController.instance.fullyLevelledWeapons.Count < PlayerController.instance.maxWeapons)
        {
            availableWeapons.AddRange(PlayerController.instance.unassignedWeapons);
        }

        for (int i = weaponsToUpgrade.Count; i < 3; i++)
        {
            if (availableWeapons.Count > 0)
            {
                int selected = Random.Range(0, availableWeapons.Count);
                weaponsToUpgrade.Add(availableWeapons[selected]);
                availableWeapons.RemoveAt(selected);
            }
        }

        for (int i = 0; i < weaponsToUpgrade.Count; i++)
        {
            UIController.instance.levelUpButtons[i].UpdateButtonDisplay(weaponsToUpgrade[i]);
        }

        for (int i = 0; i < UIController.instance.levelUpButtons.Length; i++)
        {
            if (i < weaponsToUpgrade.Count)
            {
                UIController.instance.levelUpButtons[i].gameObject.SetActive(true);
            }
            else
            {
                UIController.instance.levelUpButtons[i].gameObject.SetActive(false);
            }
        }

        PlayerStatController.instance.UpdateDisplay();
    }
}
