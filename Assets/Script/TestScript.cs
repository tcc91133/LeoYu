using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("Awake called.");
    }

    private void Update()
    {
        Debug.Log("Update called.");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key pressed!");
        }
    }
}
