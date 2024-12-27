using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    EnemyState state = EnemyState.Moving;
    float[,] markovModel = {
    //   C    M    F    L
        {0,   0,   0,   0}, // EnemyState.Corrupting
        {0,   0,   0,   0}, // EnemyState.Moving
        {0,   0,   0,   0}, // EnemyState.Fleeing
        {0,   0,   0,   0}  // EnemyState.Listening
    };
    // This code controls if the AI thinks a player is active/inactive
    [Header("Player Detection")]
    public GameObject body1, body2;
    public float SightDistance = 15f;
    public float HearingDistance = 10f;
    public float MisHearingChance = 0.1f;
    public float MaxListenTime = 2f;
    [Tooltip("The penalty used by the AI when it does bad")]
    public float penalty = -5;
    float listeningTime = 0f;
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
                listeningTime += Time.deltaTime;
                break;

            case EnemyState.Moving:
                // Movement code
                break;

            default:
                Debug.LogError("How");
                break;
        }

        UpdatePlayerStats(body1, statsArr[1]);
        UpdatePlayerStats(body2, statsArr[2]);
        SwitchStates();
    }
    
    void SwitchStates() {
        // Can't listen if they've been listening too long
        if(listeningTime > MaxListenTime) {
            listeningTime = 0f;
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
            stats.MovementDelta = failedHearing
                ? 0
                : Vector3.Distance(stats.oldPos, body.transform.position);
            stats.oldPos = failedHearing ? stats.oldPos : body.transform.position;
            
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
    Corrupting, Moving, Fleeing, Listening
}