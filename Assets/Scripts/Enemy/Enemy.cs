using System;
using UnityEngine;
using Random = UnityEngine.Random;

// using static Unity.Mathematics.math;

public class Enemy : MonoBehaviour
{
    EnemyState state = EnemyState.Moving;
    // This is kind of a markov model
    // Essentially, using the player stats array, I will change this matrix. Not really a POMDP or an HMM
    float[,] referenceModel = {
    //   C    M    F    L
        {0,   0.4f,0.0f,0.6f}, // EnemyState.Corrupting
        {0,   0.1f,0,   0.9f}, // EnemyState.Moving
        {0.05f,0.55f,0.2f,0.2f}, // EnemyState.Fleeing
        {0.01f,0.99f,0, 0}  // EnemyState.Listening
    };
    float[,] actualModel = {
    //   C    M    F    L
        {0,   0.4f,0.0f,0.6f}, // EnemyState.Corrupting
        {0,   0.1f,0,   0.9f}, // EnemyState.Moving
        {0.05f,0.5f,0.3f,  0.3f}, // EnemyState.Fleeing
        {0.01f,0.99f,0, 0}  // EnemyState.Listening
    };
    float timeClock = 0f;

    [Tooltip("This controls how long the AI has to do an action before switching")]
    public float actionTime = 1f;
    // This code controls if the AI thinks a player is active/inactive
    [Header("Player Detection")]
    public GameObject body1, body2;
    public float SightDistance = 15f;
    public float HearingDistance = 10f;
    public float MisHearingChance = 0.1f;
    public float CaughtPenalty = -0.01f;
    float eagerness = 0.2f;
    bool appliedPenalty = false;
    PlayerStats stats1 = new PlayerStats();
    PlayerStats stats2 = new PlayerStats();

    void Start()
    {
    }

    void Update()
    {
        switch (state) {
            case EnemyState.Corrupting:
                // Corrupt tile code
                break;

            case EnemyState.Fleeing:
                // Run away code
                break;

            case EnemyState.Listening:
                // Listening code
                break;

            case EnemyState.Moving:
                // Movement code
                break;

            default:
                Debug.LogError("How");
                break;
        }

        // UpdatePlayerStats(body1, ref stats1);
        UpdatePlayerStats(body2, ref stats2);
        timeClock += Time.deltaTime;
        if(timeClock > actionTime) {
            updateActualModel();
            SwitchStates();
            // Debug.Log(state);
            timeClock = 0f;
        }
    }
    
    void updateActualModel() {
        float activePercentage;
        if (stats1.visibility != PlayerVisibility.NotVisible && stats2.visibility != PlayerVisibility.NotVisible) {
            activePercentage = Mathf.Max(stats1.PercentInactive, stats2.PercentInactive);
        } else if(stats1.visibility != PlayerVisibility.NotVisible) {
            activePercentage = stats1.PercentInactive;
        } else if(stats2.visibility != PlayerVisibility.NotVisible) {
            activePercentage = stats2.PercentInactive;
        } else {
            actualModel = referenceModel;
            return;
        }

        float activePercentageCorrupting = mapRange(
            activePercentage, 
            0, 1, 
            0, 1
                -referenceModel[(int)EnemyState.Corrupting, (int)EnemyState.Listening]
                -referenceModel[(int)EnemyState.Corrupting, (int)EnemyState.Corrupting]
        );
        float max = 1
                -referenceModel[(int)EnemyState.Corrupting, (int)EnemyState.Listening]
                -referenceModel[(int)EnemyState.Corrupting, (int)EnemyState.Corrupting];
        actualModel[(int)EnemyState.Corrupting, (int)EnemyState.Moving] = max-activePercentageCorrupting;
        actualModel[(int)EnemyState.Corrupting, (int)EnemyState.Fleeing] = activePercentageCorrupting;

        float activePercentageMoving = mapRange(
            activePercentage, 
            0, 1, 
            0, 1
                -referenceModel[(int)EnemyState.Moving, (int)EnemyState.Listening]
                -referenceModel[(int)EnemyState.Moving, (int)EnemyState.Corrupting]
        );
        max = 1
                -referenceModel[(int)EnemyState.Moving, (int)EnemyState.Listening]
                -referenceModel[(int)EnemyState.Moving, (int)EnemyState.Corrupting];
        actualModel[(int)EnemyState.Moving, (int)EnemyState.Moving] = max-activePercentageMoving;
        actualModel[(int)EnemyState.Moving, (int)EnemyState.Fleeing] = activePercentageMoving;

        float activePercentageFleeing = mapRange(
            activePercentage, 
            0, 1, 
            0, 1
                -referenceModel[(int)EnemyState.Fleeing, (int)EnemyState.Listening]
                -referenceModel[(int)EnemyState.Fleeing, (int)EnemyState.Corrupting]
        );
        max = 1
                -referenceModel[(int)EnemyState.Fleeing, (int)EnemyState.Listening]
                -referenceModel[(int)EnemyState.Fleeing, (int)EnemyState.Corrupting];
        actualModel[(int)EnemyState.Fleeing, (int)EnemyState.Moving] = max-activePercentageFleeing;
        actualModel[(int)EnemyState.Fleeing, (int)EnemyState.Fleeing] = activePercentageFleeing;

        float activePercentageListening = mapRange(
            activePercentage, 
            0, 1, 
            0, 1
                -referenceModel[(int)EnemyState.Listening, (int)EnemyState.Listening]
                -referenceModel[(int)EnemyState.Listening, (int)EnemyState.Corrupting]
        );
        max = 1
                -referenceModel[(int)EnemyState.Listening, (int)EnemyState.Listening]
                -referenceModel[(int)EnemyState.Listening, (int)EnemyState.Corrupting];
        actualModel[(int)EnemyState.Listening, (int)EnemyState.Moving] = max-activePercentageListening;
        actualModel[(int)EnemyState.Listening, (int)EnemyState.Fleeing] = activePercentageListening;
        debugPrint2DArr(actualModel);
    }
    
    void debugPrint2DArr(float[,] arr) {
        String s = "";
        for(int i = 0; i < arr.GetLength(0); i++) {
            String a = "";
            for(int j = 0; j < arr.GetLength(1); j++) {
                a += " " + arr[i,j];
            }
            s += a + "\n";
        }
        Debug.Log(s);
    }

    float mapRange(float x, float in_min, float in_max, float out_min, float out_max)
    {
      return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }
    
    // I think this is it
    void SwitchStates() {
        float maxPercent = -1;
        int currentState = (int)state;
        for(int i = 0; i < 4; i++) {
            float percentCompare = actualModel[currentState,i];
            if(percentCompare>maxPercent) {
                maxPercent = percentCompare;
                state = (EnemyState)i;
            }
            // Debug.Log(percentCompare + " " + (EnemyState)i + " " + (EnemyState)currentState);
        }
    }

    void UpdatePlayerStats(GameObject body, ref PlayerStats stats) {
        float distance = Vector3.Distance(body.transform.position, transform.position);

        if(distance < HearingDistance)
            stats.visibility = PlayerVisibility.CanHear;
        else 
            stats.visibility = PlayerVisibility.NotVisible;

        if(distance < SightDistance) {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, (body.transform.position-transform.position).normalized, out hit, SightDistance) && hit.collider.CompareTag("PlayerBody")) {
                stats.visibility = PlayerVisibility.InSight;
            } 
        }
        // Debug.Log(stats.visibility);
        
        if(stats.visibility == PlayerVisibility.NotVisible) {
            appliedPenalty = false;
            return;
        }
        
        if(stats.visibility == PlayerVisibility.InSight) {
            //Always track when in sight
            stats.MovementDelta = Vector3.Distance(stats.oldPos, body.transform.position);
            stats.oldPos = body.transform.position;
            if(stats.MovementDelta>0) {
                stats.PercentInactive = 1.00f;
                if(!appliedPenalty && distance < HearingDistance) {
                    eagerness -= CaughtPenalty;
                    appliedPenalty = true;
                }
            } else {
                stats.PercentInactive -= eagerness*Time.deltaTime/10f;
                stats.PercentInactive = Mathf.Clamp(stats.PercentInactive, 0f, 1f);
            }
        } else if(state == EnemyState.Listening && stats.visibility != PlayerVisibility.NotVisible) {
            bool failedHearing = stats.visibility == PlayerVisibility.CanHear && Random.Range(0f, 1f) < MisHearingChance;
            if(!failedHearing) {
                stats.MovementDelta = Vector3.Distance(stats.oldPos, body.transform.position);
                stats.oldPos = body.transform.position;
                if(stats.MovementDelta > 0) {
                    stats.PercentInactive += eagerness*Time.deltaTime;
                    stats.PercentInactive = Mathf.Clamp(stats.PercentInactive, 0f, 1f);
                } else {
                    stats.PercentInactive -= eagerness*Time.deltaTime;
                    stats.PercentInactive = Mathf.Clamp(stats.PercentInactive, 0f, 1f);
                }
                Debug.Log(stats.MovementDelta);
            } else {
                stats.MovementDelta = 0;
            }
        }
    }
}

struct PlayerStats {
    public Vector3 oldPos;
    public float MovementDelta;
    public float PercentInactive;
    public PlayerVisibility visibility;
}

enum PlayerVisibility {
    InSight, CanHear, NotVisible
}

enum EnemyState {
    Corrupting = 0, Moving = 1, Fleeing = 2, Listening = 3
}

/*
enum POMDPState {
    Moving = -1, 
    Goal = 100, 
    Listening = -5, 
    Corrupting = -2,
    //Both of these refer to when the ai tries to run past a player cell
    Caught = -25, 
    Correct = 25,
}
*/