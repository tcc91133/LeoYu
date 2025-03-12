using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class CompleteManager : MonoBehaviour
{
    public GameObject gameCompleteUI;
    public static CompleteManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (gameCompleteUI != null)
        {
            gameCompleteUI.SetActive(false); // 默认隐藏 Game Complete UI
        }
    }

    // 检查敌人是否已经消失并且是否是最后一波
    public void CheckEnemiesCleared(bool isFinalWave)
    {
        if (isFinalWave)
        {
            StartCoroutine(WaitForAllEnemiesToClear());
        }
    }

    // 等待所有敌人消失
    private IEnumerator WaitForAllEnemiesToClear()
    {
        // 一直到场景中没有敌人为止
        while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            yield return null; // 每一帧检查一次
        }

        // 所有敌人消失后显示 Game Complete UI
        ShowGameCompleteUI();
    }

    // 显示 Game Complete UI
    private void ShowGameCompleteUI()
    {
        if (gameCompleteUI != null)
        {
            gameCompleteUI.SetActive(true); // 启用 Game Complete UI
            Time.timeScale = 0f; // 暂停游戏
        }
    }

}
