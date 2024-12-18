using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainPlayer : MonoBehaviour
{
    [Header("Camera Settings")]
    public InputAction switchCamAction;
    public Camera firstCam;
    public Camera secondCam;
    public bool isFirstCamActive = true;

    void Start()
    {
        firstCam.enabled = true;
        secondCam.enabled = false;
        switchCamAction.performed += (ctx) => {
            isFirstCamActive = !isFirstCamActive;
            SwitchCam(isFirstCamActive);
        };
    }

    void SwitchCam(bool isFirstCamActive) {
        firstCam.enabled = isFirstCamActive;
        secondCam.enabled = !isFirstCamActive;
    }

    void Update()
    {
        
    }

    void OnEnable()
    {
        switchCamAction.Enable();
    }

    void OnDisable()
    {
        switchCamAction.Disable();
    }
}
