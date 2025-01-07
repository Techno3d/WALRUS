using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shock : MonoBehaviour
{
    float health = 10;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void TakeDamage(float damage) {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other) {
        if(other.tag.Equals("Enemy")) {
            // Maybe nothing for them
        }

        if (other.tag.Equals("Player")) {
            other.GetComponent<PlayerBody>().Shock();
            Destroy(gameObject);
        }
    }
}
