using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

// If someone reaches the EndZone
public class EndZone : MonoBehaviour {
    public static event Action GameLost;
    public static event Action GameWon;
    public int DefeatedEnemies = 0;
    
    void Start()
    {
        EndZone.GameLost += () => SceneManager.LoadScene("LoseScene"); // For if you lose
        EndZone.GameWon += () => SceneManager.LoadScene("WinScene"); // For if you win
        EnemyHealth.EnemyDeath += () => DefeatedEnemies++;

    }

    void OnTriggerEnter(Collider other) {
        Settings.score = DefeatedEnemies;
        if(other.tag.Equals("Enemy")) {
            Debug.Log("we lost");
            Cursor.lockState = CursorLockMode.None;
            GameLost?.Invoke();
        }

        Debug.Log(other.tag);
        if (other.tag.Equals("Player") && Enemy.NumEnemies <= 0)
        {
            Debug.Log("we won");
            Cursor.lockState = CursorLockMode.None;
            GameWon?.Invoke();
        }
    }
}
