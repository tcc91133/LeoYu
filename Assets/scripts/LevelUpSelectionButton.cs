using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class LevelUpSelectionButton : MonoBehaviour
{
    public TMP_Text upgradeDescText, nameLevelText;  // 升级描述文本，武器名字与等级文本
    public Image weaponIcon;  // 武器图标

    private Weapon assignedWeapon;  // 分配的武器

    // 更新按钮显示内容
    public void UpdateButtonDisplay(Weapon theWeapon)
    {
        if (theWeapon.gameObject.activeSelf == true)
        {
            // 根据武器等级显示简洁的升级描述
            string upgradeText = GetUpgradeDescription(theWeapon, theWeapon.weaponLevel);
            upgradeDescText.text = upgradeText;
            weaponIcon.sprite = theWeapon.icon;

            // 显示中文等级信息
            nameLevelText.text = theWeapon.name + " - 等级 " + theWeapon.weaponLevel;
        }
        else
        {
            // 显示“解锁”信息
            upgradeDescText.text = "解鎖 -> " + theWeapon.name;
            weaponIcon.sprite = theWeapon.icon;

            nameLevelText.text = theWeapon.name;
        }
        assignedWeapon = theWeapon;
    }

    // 根据武器等级返回简洁的升级描述
    private string GetUpgradeDescription(Weapon theWeapon, int level)
    {
        string description = "";

        // 获取当前和下一级别的属性，注意索引越界的处理
        if (level < theWeapon.stats.Count - 1)  // 确保不会越界
        {
            WeaponStats currentStats = theWeapon.stats[level];  // 获取当前等级的属性
            WeaponStats nextStats = theWeapon.stats[level + 1];  // 获取下一级别的属性

            // 找到第一个有变化的属性并返回
            if (nextStats.damage != currentStats.damage)
            {
                description = $"傷害 +{nextStats.damage - currentStats.damage:F2}";
            }
            else if (nextStats.speed != currentStats.speed)
            {
                description = $"速度 +{nextStats.speed - currentStats.speed:F2}";
            }
            else if (nextStats.range != currentStats.range)
            {
                description = $"攻擊範圍 +{nextStats.range - currentStats.range:F2}";
            }
            else if (Mathf.Abs(nextStats.timeBetweenAttacks - currentStats.timeBetweenAttacks) > 0.1f)
            {
                description = $"攻擊間隔 +{(nextStats.timeBetweenAttacks - currentStats.timeBetweenAttacks > 0 ? "+" : "")}{nextStats.timeBetweenAttacks - currentStats.timeBetweenAttacks:F2}";
            }
            else if (nextStats.amount != currentStats.amount)
            {
                description = $"數量 +{nextStats.amount - currentStats.amount:F2}";
            }
            else if (nextStats.duration != currentStats.duration)
            {
                description = $"持續時間 +{nextStats.duration - currentStats.duration:F2}";
            }
        }
        else
        {
            description = "已升滿級";  // 默认描述
        }

        return description;
    }

    // 选择升级时的操作
    public void SelectUpgrade()
    {
        if (assignedWeapon != null)
        {
            if (assignedWeapon.gameObject.activeSelf == true)
            {
                // 升级武器
                assignedWeapon.Levelup();
                SFXManager.instance.PlaySFXPitched(12);  // 播放音效
            }
            else
            {
                // 解锁武器
                PlayerController.instance.AddWeapon(assignedWeapon);
                SFXManager.instance.PlaySFXPitched(12);  // 播放音效
            }

            // 隐藏升级面板
            UIController.instance.levelUpPanel.SetActive(false);
            Time.timeScale = 1f;  // 恢复正常游戏速度
        }
    }
}
