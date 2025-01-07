using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
    [SerializeField] Text text;

    AudioManager audioManager;
    private void Awake(){
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    void Start()
    {
        audioManager.PlaySFX(audioManager.lose);
        text.text = Settings.score + "/" + Enemy.TotalNumEnemies + " DEFEATED";
    }
}
