using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShockedHelpText : MonoBehaviour
{
    TMP_Text text;
    [SerializeField]
    MainPlayer mainPlayer;
    String actualText = @"
You are currently 'Shocked!'
You cannot move for __ seconds
You can still switch to your other body
";
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mainPlayer.ActiveBody.isShocked) {
            text.text = actualText.Replace("__", ""+(int)mainPlayer.ActiveBody.TimeLeft);
        }
        else {
            text.text = "";
        }
    }
}
