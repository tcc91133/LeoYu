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
        valueText.text = "�ƭ� " + oldValue.ToString("F1") + "->" + newValue.ToString("F1");
        costText.text = "��O: " + cost;

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
        valueText.text = "����";
        costText.text = "����";
        upgradeButton.SetActive(false);
    }


}
