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
        {0,   1.0f,0.0f,0}, // EnemyState.Corrupting
        {0,   0.9f,0,   0.1f}, // EnemyState.Moving
        {0.05f,0.5f,0.3f,  0.3f}, // EnemyState.Fleeing
        {0.01f,0.99f,0, 0}  // EnemyState.Listening
    };
    float[,] actualModel = {
    //   C    M    F    L
        {0,   0.5f,0.5f,0}, // EnemyState.Corrupting
        {0,   0.9f,0,   0.1f}, // EnemyState.Moving
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
    PlayerStats[] statsArr = new PlayerStats[2];

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

        timeClock += Time.deltaTime;
        if(timeClock > actionTime) {
            UpdatePlayerStats(body1, statsArr[1]);
            UpdatePlayerStats(body2, statsArr[2]);
            updateActualModel();
            SwitchStates();
            timeClock = 0f;
        }
    }
    
    void updateActualModel() {
        float activePercentage = -1f;
        if(statsArr[0].visibility != PlayerVisibility.NotVisible && statsArr[1].visibility != PlayerVisibility.NotVisible) {
            activePercentage = Mathf.Max(statsArr[0].PercentInactive, statsArr[1].PercentInactive);
        } else if(statsArr[0].visibility != PlayerVisibility.NotVisible) {
            activePercentage = statsArr[0].PercentInactive;
        } else if(statsArr[1].visibility != PlayerVisibility.NotVisible) {
            activePercentage = statsArr[1].PercentInactive;
        } else {
            actualModel = referenceModel;
            return;
        }

        float activePercentageCorrupting = mapRange(
            activePercentage, 
            0, 1, 
            0, 1
                -actualModel[(int)EnemyState.Corrupting, (int)EnemyState.Listening]
                -actualModel[(int)EnemyState.Corrupting, (int)EnemyState.Corrupting]
        );
        actualModel[(int)EnemyState.Corrupting, (int)EnemyState.Moving] = 1-activePercentageCorrupting;
        actualModel[(int)EnemyState.Corrupting, (int)EnemyState.Fleeing] = activePercentageCorrupting;

        float activePercentageMoving = mapRange(
            activePercentage, 
            0, 1, 
            0, 1
                -actualModel[(int)EnemyState.Moving, (int)EnemyState.Listening]
                -actualModel[(int)EnemyState.Moving, (int)EnemyState.Corrupting]
        );
        actualModel[(int)EnemyState.Moving, (int)EnemyState.Moving] = 1-activePercentageMoving;
        actualModel[(int)EnemyState.Moving, (int)EnemyState.Fleeing] = activePercentageMoving;

        float activePercentageFleeing = mapRange(
            activePercentage, 
            0, 1, 
            0, 1
                -actualModel[(int)EnemyState.Fleeing, (int)EnemyState.Listening]
                -actualModel[(int)EnemyState.Fleeing, (int)EnemyState.Corrupting]
        );
        actualModel[(int)EnemyState.Fleeing, (int)EnemyState.Moving] = 1-activePercentageFleeing;
        actualModel[(int)EnemyState.Fleeing, (int)EnemyState.Fleeing] = activePercentageFleeing;

        float activePercentageListening = mapRange(
            activePercentage, 
            0, 1, 
            0, 1
                -actualModel[(int)EnemyState.Listening, (int)EnemyState.Listening]
                -actualModel[(int)EnemyState.Listening, (int)EnemyState.Corrupting]
        );
        actualModel[(int)EnemyState.Listening, (int)EnemyState.Moving] = 1-activePercentageListening;
        actualModel[(int)EnemyState.Listening, (int)EnemyState.Fleeing] = activePercentageListening;
    }

    float mapRange(float x, float in_min, float in_max, float out_min, float out_max)
    {
      return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }
    
    // I think this is it
    void SwitchStates() {
        float maxPercent = -1;
        for(int i = 0; i < 4; i++) {
            float percentCompare = actualModel[(int)state,i];
            if(percentCompare>maxPercent) {
                maxPercent = percentCompare;
                state = (EnemyState)i;
            }
        }
    }

    void UpdatePlayerStats(GameObject body, PlayerStats stats) {
        float distance = Vector3.Distance(body.transform.position, transform.position);

        if(distance < HearingDistance)
            stats.visibility = PlayerVisibility.CanHear;
        else 
            stats.visibility = PlayerVisibility.NotVisible;

        if(distance < SightDistance) {
            if(!Physics.Raycast(transform.position, (body.transform.position-transform.position).normalized, SightDistance)) {
                stats.visibility = PlayerVisibility.InSight;
            } 
        }
        
        if(stats.visibility == PlayerVisibility.NotVisible)
            return;
        
        if(stats.visibility == PlayerVisibility.InSight) {
            //Always track when in sight
            stats.MovementDelta = Vector3.Distance(stats.oldPos, body.transform.position);
            stats.oldPos = body.transform.position;
            if(stats.MovementDelta>0)
                stats.PercentInactive = 1.00f;
        } else if(state == EnemyState.Listening && stats.visibility != PlayerVisibility.NotVisible) {
            bool failedHearing = stats.visibility == PlayerVisibility.CanHear && Random.Range(0f, 1f) < MisHearingChance;
            if(!failedHearing) {
                stats.MovementDelta = Vector3.Distance(stats.oldPos, body.transform.position);
                stats.oldPos = body.transform.position;
                if(stats.MovementDelta > 0) {
                    Debug.Log(stats.MovementDelta);
                    stats.PercentInactive += 0.2f;
                    stats.PercentInactive = Mathf.Clamp(stats.PercentInactive, 0f, 1f);
                } else {
                    stats.PercentInactive -= 0.2f;
                    stats.PercentInactive = Mathf.Clamp(stats.PercentInactive, 0f, 1f);
                }
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