using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    float health;
    // Used when damage was taken
    // public static event Action<float> HealthChanged;
    // Probably going to be used by a HUD UI
    public static event Action EnemyDeath;
    
    public void TakeDamage(float damage) {
        health -= damage;
        if(health <= 0) {
            EnemyDeath?.Invoke();
        }
    }
}
