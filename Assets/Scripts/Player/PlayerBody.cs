using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBody : MonoBehaviour
{
    [Header("Link Settings")]
    public Camera cam;
    public AudioListener audioListener;
    public CharacterController controller;
    public MouseControl mouseControl;
    public ApplyGravity applyGravity;
    public GameObject TheRealModel;
    public GameObject BeamProjector;

    [Header("Speed Settings")]
    public float speed = 12f;
    public float gravity = 9.81f;
    public float maxAirTime = 0.2f;


    [Header("Attack Settings")]
    public GameObject beam;
    public float BeamRange = 5f;
    [NonSerialized]
    public bool isFiringBeam = false;

    private Vector3 velocity = Vector3.zero;
    private GameControls controls;
    float airTime = 0f;
    bool wasHeld = false;
    static float damage = 3;
    
    [System.NonSerialized]
    public bool isShocked = false;
    float timeClock = 0f;
    public float TimeLeft => 10-timeClock;

    AudioManager audioManager;

    void Awake() {
        controls = new GameControls();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        applyGravity = GetComponent<ApplyGravity>();
        applyGravity.gravity = gravity;
        applyGravity.controller = controller;
        applyGravity.enabled = false;
    }

    void Update()
    {
        if(timeClock > 10) {
            isShocked = false;
        }
        if(isShocked) {
            timeClock += Time.deltaTime;
            return;
        }
        Vector2 movement = controls.Player.Movement.ReadValue<Vector2>();
        Vector3 move = transform.right * movement.x + transform.forward * movement.y;
        move = Vector3.ClampMagnitude(move, 1);
        velocity.x = Mathf.MoveTowards(velocity.x, move.x*speed, 10f);
        velocity.z = Mathf.MoveTowards(velocity.z, move.z*speed, 10f);

        // THis is for jump
        if(controls.Player.Jump.IsPressed() && airTime < maxAirTime && wasHeld) {
            velocity.y += gravity*Time.deltaTime * (airTime == 0 ? 50 : 1);
            airTime += Time.deltaTime;
        } else if(controller.isGrounded) {
            velocity.y = 0;
            airTime = 0;
        } else {
            velocity.y += -gravity * Time.deltaTime;
        }
        
        // This is for variable jump height
        if(controls.Player.Jump.IsPressed() && controller.isGrounded) {
            wasHeld = true;
        } else if (!controls.Player.Jump.IsPressed() && !controller.isGrounded) {
            wasHeld = false;
        }
        
        // Final velocity for movement
        controller.Move(velocity * Time.deltaTime);
        
        if(controls.Player.Attack.IsPressed()) {

            beam.SetActive(true);
            BeamProjector.SetActive(true);
            BeamProjector.transform.localRotation = Quaternion.Euler(
                -2,
                -15,
                0
            );
            AttackBeam();
            if (!audioManager.IsSFXPlaying())
    {
        audioManager.PlaySFX(audioManager.playerShoot);
    }
        } else {
           beam.SetActive(false);
    BeamProjector.SetActive(false);
    audioManager.StopSFX();
        }
    }

    // Laser attack sound can go here
    void AttackBeam()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, BeamRange)) {
            beam.transform.localScale = new Vector3(1, 1, hit.distance);
            beam.transform.localRotation = Quaternion.Euler(cam.transform.localEulerAngles.x, Mathf.Atan2(hit.distance, 0.7f)*Mathf.Rad2Deg-90, 0);
            float funnyDamage = damage*Time.deltaTime*(1-hit.distance/(3*BeamRange));
            if(hit.collider.CompareTag("Enemy")) {
                hit.collider.GetComponent<EnemyHealth>().TakeDamage(funnyDamage);
            } else if(hit.collider.CompareTag("CorruptionCube")) {
                hit.collider.GetComponent<CorruptionHealth>().TakeDamage(funnyDamage);
            } else if(hit.collider.CompareTag("ShockedFloor")) {
                hit.collider.GetComponentInParent<Shock>().TakeDamage(funnyDamage);
            }

        } else {
            beam.transform.localScale = new Vector3(1, 1, BeamRange);
            beam.transform.localRotation = Quaternion.Euler(cam.transform.localEulerAngles.x, Mathf.Atan2(BeamRange, 0.7f)*Mathf.Rad2Deg-90, 0);
        }
    }

    void OnEnable() {
        controls.Enable();
        cam.enabled = true;
        audioListener.enabled = true;
        mouseControl.enabled = true;
        velocity = applyGravity.velocity;
        timeClock = applyGravity.timeClock;
        applyGravity.enabled = false;
        EnemyHealth.EnemyDeath += Analyze;
        TheRealModel.SetActive(false);
    }

    void OnDisable() {
        controls.Disable();
        cam.enabled = false;
        audioListener.enabled = false;
        mouseControl.enabled = false;
        applyGravity.enabled = true;
        applyGravity.velocity = velocity;
        applyGravity.timeClock = timeClock;
        beam.SetActive(false);
        EnemyHealth.EnemyDeath -= Analyze;
        TheRealModel.SetActive(true);
    }
    
    void Analyze() => damage++;
    
    public void Shock() {
        isShocked = true;
        timeClock = 0f;
    }
}
