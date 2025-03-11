using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryDestroyer : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"Object {other.gameObject.name} exited the boundary and was destroyed.");
        Destroy(other.gameObject); 
    }
}
