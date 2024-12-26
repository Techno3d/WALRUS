using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseControl : MonoBehaviour
{
    public Transform playerBody;
    public float mouseSensativity = 100f;
    private GameControls controls;
    private float xRotation = 0;

    void Awake()
    {
        controls = new GameControls();
    }

    void Update()
    {
        Vector2 deltas = controls.Player.MouseControl.ReadValue<Vector2>() * mouseSensativity * Time.deltaTime;
        xRotation -= deltas.y;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerBody.Rotate(Vector3.up * deltas.x);
    }
    
    void OnEnable() {
        controls.Enable();
    }

    void OnDisable() {
        controls.Disable();
    }
}
