using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthController : MonoBehaviour
{
    public static PlayerHealthController instance;

    private void Awake()
    {
        instance = this;
    }

    public float currentHealth, maxHealth;
    // Start is called before the first frame update

    public Slider healthSlider;

    public GameObject deathEffect;

    void Start()
    {
        //maxHealth = PlayerStatController.instance.health[0].value;
        currentHealth = maxHealth;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    // Update is called once per frame
    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) {
            TakeDamage(10f);
        }

    }*/

    public void TakeDamage(float damageToTake)
    {
        currentHealth -= damageToTake;

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);

            LevelManager.instance.EndLevel();

            Instantiate(deathEffect, transform.position, transform.rotation);

            //SFXManager.instance.PlaySFX(3);
        }

        healthSlider.value = currentHealth;
    }
}