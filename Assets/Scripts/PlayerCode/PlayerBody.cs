using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    [Header("Link Settings")]
    public Camera cam;
    public AudioListener audioListener;
    public CharacterController controller;

    [Header("Speed Settings")]
    public float speed = 12f;
    public float gravity = 9.81f;
    public float maxAirTime = 0.2f;

    private Vector3 velocity = Vector3.zero;
    private GameControls controls;
    float airTime = 0f;
    bool wasHeld = false;

    void Awake() {
        controls = new GameControls();
    }

    void Start()
    {
        
    }

    void Update()
    {
        Vector2 movement = controls.Player.Movement.ReadValue<Vector2>();
        Vector3 move = transform.right * movement.x + transform.forward * movement.y;
        move = Vector3.ClampMagnitude(move, 1);
        velocity.x = Mathf.MoveTowards(velocity.x, move.x*speed, 10f);
        velocity.z = Mathf.MoveTowards(velocity.z, move.z*speed, 10f);

        if(controls.Player.Jump.IsPressed() && airTime < maxAirTime && wasHeld) {
            velocity.y += gravity*Time.deltaTime * (airTime == 0 ? 50 : 1);
            airTime += Time.deltaTime;
        } else if(controller.isGrounded) {
            velocity.y = 0;
            airTime = 0;
        } else {
            velocity.y += -gravity * Time.deltaTime;
        }
        
        if(controls.Player.Jump.IsPressed() && controller.isGrounded) {
            wasHeld = true;
        } else if (!controls.Player.Jump.IsPressed() && !controller.isGrounded) {
            wasHeld = false;
        }
        
        controller.Move(velocity * Time.deltaTime);
    }

    void OnEnable() {
        controls.Enable();
        cam.enabled = true;
        audioListener.enabled = true;
    }
    
    void OnDisable() {
        controls.Disable();
        cam.enabled = false;
        audioListener.enabled = false;
    }
}
