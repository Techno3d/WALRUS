using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyGravity : MonoBehaviour
{
    public float gravity = 9.81f;
    public CharacterController controller;
    public  Vector3 velocity = Vector3.zero;

    void Update()
    {
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
