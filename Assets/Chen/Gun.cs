using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    protected Vector2 mousePos;       // Mouse position in world space
    protected Camera mainCamera;      // Reference to the main camera

    protected virtual void Start()
    {
        // Cache the main camera for efficiency
        mainCamera = Camera.main;
    }

    protected virtual void Update()
    {
        // Get the mouse position in world space
        mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // Calculate the direction from gun to mouse
        Vector2 direction = mousePos - (Vector2)transform.position;

        // Calculate the angle to rotate the gun
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply the rotation based on the angle
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Flip the gun's scale horizontally if the mouse is on the left side
        if (mousePos.x < transform.position.x)
        {
            // Flip the gun to face the left
            transform.localScale = new Vector3(1, -1, 1); // Flip on Y-axis to mirror horizontally
        }
        else
        {
            // Face right (default direction)
            transform.localScale = new Vector3(1, 1, 1); // Reset flip when on the right
        }
    }
}






/*if (Input.GetButton("Fire1"))
{
    if (timer == 0)
    {
        timer = interval;
        //Fire();
    }
}*/


/*protected virtual void Fire()
{
    animator.SetTrigger("Shoot");

    GameObject bullet = Instantiate(bulletPrefab, muzzlePos.position, Quaternion.identity);
    //GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab);
    bullet.transform.position = muzzlePos.position;

    float angel = Random.Range(-5f, 5f);
    //bullet.GetComponent<Bullet>().SetSpeed(Quaternion.AngleAxis(angel, Vector3.forward) * direction);
    bullet.GetComponent<Bullet>().SetSpeed(direction);
    Instantiate(shellPrefab, shellPos.position, shellPos.rotation);
    //GameObject shell = ObjectPool.Instance.GetObject(shellPrefab);
    //shell.transform.position = shellPos.position;
    //shell.transform.rotation = shellPos.rotation;
}*/

