using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

// If someone reaches the EndZone
public class EndZone : MonoBehaviour {
    public static event Action GameLost;
    public static event Action GameWon;
    public static int DefeatedEnemies = 0;
    
    void Start()
    {
        EndZone.GameLost += () => SceneManager.LoadScene("LoseScene"); // For if you lose
        EndZone.GameWon += () => SceneManager.LoadScene("WinScene"); // For if you win
        EnemyHealth.EnemyDeath += () => DefeatedEnemies++;

    }

    void OnTriggerEnter(Collider other) {
        if(other.tag.Equals("Enemy")) {
            Debug.Log("we lost");
            GameLost?.Invoke();
        }

        if (other.tag.Equals("PlayerMan") && Enemy.NumEnemies <= 0)
        {
            Debug.Log("we won");
            GameWon?.Invoke();
        }
    }
}
