using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private GameObject deathUI;
    [SerializeField] private UnityEvent onDeath; 
    private Health healthComponent;

    private void Start()
    {
        healthComponent = GetComponent<Health>();
        deathUI.SetActive(false);
        Time.timeScale = 1; 
    }

    private void Update()
    {
        CheckDeath();
    }

    private void CheckDeath()
    {
        if (healthComponent.Value <= 0)
            Die();
    }

    private void Die()
    {
        deathUI.SetActive(true);
        Time.timeScale = 0; 
        onDeath.Invoke(); 
    }

    public void RestartGame()
    {
        Time.timeScale = 1; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }
}
