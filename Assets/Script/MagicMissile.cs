using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timers;

public class MagicMissile : MonoBehaviour
{
    [SerializeField] private MissileCreator creator;

    private void LaunchMissile()
    {
        creator.CreateMissile();
    }

    private void Update()
    {
        // 當玩家按下空白鍵時發射子彈
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LaunchMissile();
        }
    }
}