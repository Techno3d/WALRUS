using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // This code controls if the AI thinks a player is active/inactive
    [Header("Player Detection")]
    public GameObject body1, body2;
    public float SightDistance = 15f;
    public float HearingDistance = 10f;
    public float MisHearingChance = 0.1f;
    [Tooltip("The penalty used by the AI when it does bad")]
    public float penalty = -5;
    PlayerStats[] statsArr = new PlayerStats[2];

    void Start()
    {
    }

    void Update()
    {
        
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
        
        stats.MovementDelta = (stats.visibility == PlayerVisibility.CanHear && Random.Range(0f, 1f) < MisHearingChance)
            ? 0
            : Vector3.Distance(stats.oldPos, body.transform.position);
        stats.oldPos = body.transform.position;
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