using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionHealth : MonoBehaviour
{
    [SerializeField]
    float health;
    
    public void TakeDamage(float damage) {
        health -= damage;
        if(health <= 0) {
            Destroy(this.gameObject);
        }
    }
}
