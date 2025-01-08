using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is so you don't float if you switch bodies
public class ApplyGravity : MonoBehaviour
{
    public float gravity = 9.81f;
    public CharacterController controller;
    public  Vector3 velocity = Vector3.zero;
    public float timeClock;

    void Update()
    {
        timeClock += Time.deltaTime;
        velocity.x = 0f;
        velocity.z = 0f;
        if(controller == null) 
            return;
        if(controller.isGrounded) {
            velocity.y = 0;
        } else {
            velocity.y += -gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }
}
