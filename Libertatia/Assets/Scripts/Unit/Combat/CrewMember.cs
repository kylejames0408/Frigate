using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewMember : Character
{
    public List<Character> enemies;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 100;
        currentHealth = 100;
        attackRange = 2;
        attackRate = 2;
        damage = 50;
    }

    // Update is called once per frame
    void Update()
    {
        Attack(enemies);
        healthbar.UpdateHealthBar(maxHealth, currentHealth);
        Death();
    }
}
