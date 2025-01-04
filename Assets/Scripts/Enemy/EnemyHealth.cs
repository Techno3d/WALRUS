using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    float health;
    // Used when damage was taken
    // public static event Action<float> HealthChanged;
    // Probably going to be used by a HUD UI
    // Also could hook up to sound, unless you put the sound into the if statement in the takedamage script
    public static event Action EnemyDeath;
    
    public void TakeDamage(float damage) {
        health -= damage;
        if(health <= 0) {
            EnemyDeath?.Invoke();
            // Destroy(gameObject);
        }
    }
}
