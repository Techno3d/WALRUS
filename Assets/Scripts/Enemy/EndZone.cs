using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemy has reached EndZone
public class EndZone : MonoBehaviour {
    public static event Action GameLost;

    void OnTriggerEnter(Collider other) {
        if(other.tag.Equals("Enemy")) {
            GameLost?.Invoke();
        }
    }
}
