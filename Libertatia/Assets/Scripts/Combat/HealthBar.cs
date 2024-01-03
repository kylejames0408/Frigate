using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarSprite;

    [SerializeField] private float reduceSpeed = 2;

    private float target = 1;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        target = currentHealth/maxHealth;
    }

    private void Update()
    {
        //Makes the health bar always face the camera
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
        healthBarSprite.fillAmount = Mathf.MoveTowards(healthBarSprite.fillAmount, target, reduceSpeed * Time.deltaTime);
    }
}
