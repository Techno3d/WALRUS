using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScript : MonoBehaviour
{
    [SerializeField] Text text;
    void Start()
    {
        text.text = Settings.score + "/" + Enemy.TotalNumEnemies + " DEFEATED";
    }
}
