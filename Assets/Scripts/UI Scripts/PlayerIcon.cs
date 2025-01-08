using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIcon : MonoBehaviour
{
    public bool isDisabled;
    Color originalColor;
    Color disabled;
    void Start()
    {
        originalColor = GetComponent<Image>().color;
        disabled = originalColor;
        disabled.a = 0;
        Switch();
        Switch();
    }
    
    void OnEnable() {
        MainPlayer.SwitchedBody += Switch;
    }

    void OnDisable() {
        MainPlayer.SwitchedBody -= Switch;
    }

    void Switch() {
        isDisabled = !isDisabled;
        if(isDisabled)
            GetComponent<Image>().color = originalColor;
        else
            GetComponent<Image>().color = disabled;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
