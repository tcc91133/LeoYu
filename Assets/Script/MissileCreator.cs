using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timers;

public class MissileCreator : MonoBehaviour
{
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private Transform playerTransform;

    public void CreateMissile()
    {
        Instantiate(missilePrefab, playerTransform.position, Quaternion.identity);
    }
}
