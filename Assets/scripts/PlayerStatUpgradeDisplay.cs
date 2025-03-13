using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerStatUpgradeDisplay : MonoBehaviour
{
    public TMP_Text valueText,costText;
    public GameObject upgradeButton;

    public void UpdateDisplay(int cost, float oldValue,float newValue)
    {
        valueText.text = "數值 " + oldValue.ToString("F1") + "->" + newValue.ToString("F1");
        costText.text = "花費: " + cost;

        if(cost <= CoinController.instance.currentCoins)
        {
            upgradeButton.SetActive(true);
        }else
        {
            upgradeButton.SetActive(false);
        }
    }

    public void ShowMaxLevel()
    {
        valueText.text = "滿級";
        costText.text = "滿級";
        upgradeButton.SetActive(false);
    }


}
