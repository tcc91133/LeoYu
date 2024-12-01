using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpSelectionButton : MonoBehaviour
{
    public TMP_Text upgradeDescText, nameLevelText;
    public Image weaponIcon;

    private Weapon assignedWeapon;

    public void UpdataButtonDisplay(Weapon theWeapon)
    {
        upgradeDescText.text = theWeapon.stats[theWeapon.weaponLevel].upgradeText;
        weaponIcon.sprite = theWeapon.icon;

        nameLevelText.text = theWeapon.name + " - Lvl " + theWeapon.weaponLevel;
        assignedWeapon = theWeapon;
    }
    /*if(theWeapon.gameObject.activeSelf == true)
    {

    }else
    {
        upgradeDescText.text = "Unlock " + theWeapon.name;
        weaponIcon.sprite = theWeapon.icon;

        nameLevelText.text = theWeapon.name;
    }
    assignWeapon = theWeapon;*/


    public void SelectUpgrade()
    {
        if(assignedWeapon != null)
        {
            /*if(assignWeapon.gameObject.activeSelf == true)
            {*/
                assignedWeapon.Levelup();
            /*}else
            {
                PlayerController.instance.AddWeapon(assignWeapon);
            }*/
            

            UIController.Instance.levelUpPanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

}
