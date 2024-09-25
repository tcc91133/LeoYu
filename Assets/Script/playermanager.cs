using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playermanager : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    private static playermanager _instance;

    public static Vector2 Position
    {
        get { return _instance.playerTransform.position; }
    }
    private void Awake()
    {
        _instance = this;
    }
}
