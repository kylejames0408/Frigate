using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarSprite;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    /// <summary>
    /// Updates health bar based on the unit's current and maximum health
    /// </summary>
    /// <param name="maxHealth"></param>
    /// <param name="currentHealth"></param>
    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        healthBarSprite.fillAmount = currentHealth/maxHealth;
    }

    private void Update()
    {
        //Makes the health bar always face the camera
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
    }
}
