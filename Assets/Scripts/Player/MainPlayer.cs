using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainPlayer : MonoBehaviour
{
    [Header("Body Settings")]
    public PlayerBody FirstBody;
    public PlayerBody SecondBody;
    private bool isFirstActive = true;
    private GameControls controls;
    public static event Action SwitchedBody;

    void Awake() {
        controls = new GameControls();
    }

    void Start()
    {
        SwitchBody(isFirstActive);
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnEnable() {
        controls.Enable();
        controls.Player.SwitchCam.performed += (ctx) => {
            isFirstActive = !isFirstActive;
            SwitchBody(isFirstActive);
        };
    }

    void SwitchBody(bool isFirstCamActive) {
        FirstBody.enabled = isFirstCamActive;
        SecondBody.enabled = !isFirstCamActive;
    }

    void Update()
    {
        
    }
}
