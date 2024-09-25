using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f; // 子彈存活時間，預設為5秒

    private void Start()
    {
        // 在指定的時間後銷毀子彈
        Destroy(gameObject, lifetime);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
