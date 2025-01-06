using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    float MaxHealth;
    [System.NonSerialized]
    public float health;

    public Slider healthBar;
    // Used when damage was taken
    // public static event Action<float> HealthChanged;
    // Probably going to be used by a HUD UI
    // Also could hook up to sound, unless you put the sound into the if statement in the takedamage script
    public static event Action EnemyDeath;

    void Start()
    {
        health = MaxHealth;
        healthBar.maxValue = health;
        healthBar.value = health;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.value = health;
        if (health <= 0)
        {
            EnemyDeath?.Invoke();
            Destroy(gameObject);
        }
    }
}
