using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainPlayer : MonoBehaviour
{
    [Header("Body Settings")]
    public GameObject FirstBody;
    public GameObject SecondBody;
    private bool isFirstActive = true;
    private GameControls controls;

    void Awake() {
        controls = new GameControls();
    }

    void Start()
    {
        FirstBody.SetActive(true);
        SecondBody.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnEnable() {
        controls.Enable();
        controls.Player.SwitchCam.performed += (ctx) => {
            isFirstActive = !isFirstActive;
            SwitchCam(isFirstActive);
        };
    }

    void SwitchCam(bool isFirstCamActive) {
        FirstBody.SetActive(isFirstCamActive);
        SecondBody.SetActive(!isFirstCamActive);
    }

    void Update()
    {
        
    }
}
