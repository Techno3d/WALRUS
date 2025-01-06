using System;
using UnityEngine;
using Random = UnityEngine.Random;

// using static Unity.Mathematics.math;

public class Enemy : MonoBehaviour
{
    public static float TotalNumEnemies = 0;
    public static float NumEnemies = 0;
    EnemyState state = EnemyState.Moving;
    // This is kind of a markov model
    // Essentially, using the player stats array, I will change this matrix. Not really a POMDP or an HMM
    float[,] referenceModel = {
    //   C    M    F    L
        {0,   0.4f,0.0f,0.6f}, // EnemyState.Corrupting
        {0,   0.1f,0,   0.4f}, // EnemyState.Moving
        {0.05f,0.55f,0.2f,0.2f}, // EnemyState.Fleeing
        {0.01f,0.99f,0, 0}  // EnemyState.Listening
    };
    float[,] actualModel;
    float timeClock = 0f;
    public Vector3 target = Vector3.zero;

    [Header("Attacks")]
    public GameObject corruptionCube;
    public float corruptionTime = 0.5f, movingTime = 2f, fleeingTime = 2f, listeningTime = 0.5f;
    // [Tooltip("This controls how long the AI has to do an action before switching")]
    float actionTime => state switch
    {
        EnemyState.Corrupting => corruptionTime,
        EnemyState.Moving => movingTime,
        EnemyState.Fleeing => fleeingTime,
        EnemyState.Listening => listeningTime,
        _ => 1f,
    };

    // This code controls if the AI thinks a player is active/inactive
    [Header("Player Detection")]
    public GameObject body1;
    public GameObject body2;
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
        actualModel = referenceModel;
        target = transform.position;
        TotalNumEnemies++;
        NumEnemies++;
    }

    void Update()
    {
        switch (state)
        {
            case EnemyState.Corrupting:
                break;

            case EnemyState.Fleeing:
                break;

            case EnemyState.Listening:
                break;

            case EnemyState.Moving:
                break;

            default:
                Debug.LogError("How");
                break;
        }
        // Update the stats
        UpdatePlayerStats(body1, ref stats1);
        UpdatePlayerStats(body2, ref stats2);
        timeClock += Time.deltaTime;
        if (timeClock > actionTime)
        {
            EnemyState prevState = state;
            updateActualModel();
            SwitchStates();

            // This state switch is for things that happen on a switch
            switch (state)
            {
                case EnemyState.Corrupting:
                    target = transform.position;
                    CorruptTile();
                    break;

                case EnemyState.Fleeing:
                    Flee();
                    break;

                case EnemyState.Listening:
                    target = transform.position;
                    break;

                case EnemyState.Moving:
                    if(prevState == EnemyState.Moving && Vector3.Distance(transform.position, target) < 2) {
                        // Do nothing?
                    } else {
                        Move();
                    }
                    break;

                default:
                    Debug.LogError("How");
                    break;
            }
            timeClock = 0f;
        }
    }

    void Move()
    {
        Vector3 direction = Vector3.one - Vector3.up;
        direction *= 6 * Random.Range(1,3);
        target.x += direction.x;
        target.z += direction.z;
        target.x = Mathf.Clamp(target.x, 0, 97);
        target.z = Mathf.Clamp(target.z, 0, 97);
    }

    void Flee()
    {
        GameObject activeBody;
        if (stats1.PercentInactive > stats2.PercentInactive)
        {
            activeBody = body1;
        }
        else
        {
            activeBody = body2;
        }

        Vector2 dir = new Vector2((transform.position - activeBody.transform.position).x, (transform.position - activeBody.transform.position).z);
        dir.Normalize();
        dir.x = Mathf.Round(dir.x);
        dir.y = Mathf.Round(dir.y);
        dir *= 6;

        target.x += dir.x;
        target.z += dir.y;
        target.x = Mathf.Clamp(target.x, 0, 97);
        target.z = Mathf.Clamp(target.z, 0, 97);
    }

    void Listen()
    {
        // Provides a break to enhance humanlike behavior
    }

    void CorruptTile()
    {
        GameObject activeBody;
        if (stats1.PercentInactive > stats2.PercentInactive)
        {
            activeBody = body1;
        }
        else
        {
            activeBody = body2;
        }
        // Direction to go in
        Vector2 dir = new Vector2((activeBody.transform.position - transform.position).x, (activeBody.transform.position - transform.position).z);
        dir.Normalize();
        dir.x = Mathf.Round(dir.x);
        dir.y = Mathf.Round(dir.y);
        dir *= 6;

        //Where AI is, but centered on tile
        Vector2 roundedPos = new Vector2(transform.position.x - 3, transform.position.z - 3);
        roundedPos /= 3;
        roundedPos.x = Mathf.Round(roundedPos.x);
        roundedPos.y = Mathf.Round(roundedPos.y);
        roundedPos *= 3;

        Vector2 spawnPos = roundedPos + dir;
        Instantiate(corruptionCube, new Vector3(spawnPos.x, 0.5f, spawnPos.y), Quaternion.identity);
        stats1.PercentInactive = Mathf.Clamp(stats1.PercentInactive - 0.4f, 0, 1);
        stats2.PercentInactive = Mathf.Clamp(stats2.PercentInactive - 0.4f, 0, 1);
    }

    // This code updates the model based on the observation
    void updateActualModel()
    {
        float activePercentage;
        // Does the AI even see/hear the player?
        if (stats1.visibility != PlayerVisibility.NotVisible && stats2.visibility != PlayerVisibility.NotVisible)
        {
            activePercentage = Mathf.Max(stats1.PercentInactive, stats2.PercentInactive);
        }
        else if (stats1.visibility != PlayerVisibility.NotVisible)
        {
            activePercentage = stats1.PercentInactive;
        }
        else if (stats2.visibility != PlayerVisibility.NotVisible)
        {
            activePercentage = stats2.PercentInactive;
        }
        else
        {
            // Could be cool to slowly make reference model into actual, but this should still work
            actualModel = referenceModel;
            return;
        }

        // The rest of this code just makes sure the percentages add up to 100%, else we have problems
        // The map function makes sure that the movement/fleeing percent will add up
        float activePercentageCorrupting = mapRange(
            activePercentage,
            0, 1,
            0, 1
                - referenceModel[(int)EnemyState.Corrupting, (int)EnemyState.Listening]
                - referenceModel[(int)EnemyState.Corrupting, (int)EnemyState.Corrupting]
        );
        float max = 1
                - referenceModel[(int)EnemyState.Corrupting, (int)EnemyState.Listening]
                - referenceModel[(int)EnemyState.Corrupting, (int)EnemyState.Corrupting];
        actualModel[(int)EnemyState.Corrupting, (int)EnemyState.Moving] = max - activePercentageCorrupting;
        actualModel[(int)EnemyState.Corrupting, (int)EnemyState.Fleeing] = activePercentageCorrupting;

        float activePercentageMoving = mapRange(
            activePercentage,
            0, 1,
            0, 1
                - referenceModel[(int)EnemyState.Moving, (int)EnemyState.Listening]
                - referenceModel[(int)EnemyState.Moving, (int)EnemyState.Corrupting]
        );
        max = 1
                - referenceModel[(int)EnemyState.Moving, (int)EnemyState.Listening]
                - referenceModel[(int)EnemyState.Moving, (int)EnemyState.Corrupting];
        actualModel[(int)EnemyState.Moving, (int)EnemyState.Moving] = max - activePercentageMoving;
        actualModel[(int)EnemyState.Moving, (int)EnemyState.Fleeing] = activePercentageMoving;

        float activePercentageFleeing = mapRange(
            activePercentage,
            0, 1,
            0, 1
                - referenceModel[(int)EnemyState.Fleeing, (int)EnemyState.Listening]
                - referenceModel[(int)EnemyState.Fleeing, (int)EnemyState.Corrupting]
        );
        max = 1
                - referenceModel[(int)EnemyState.Fleeing, (int)EnemyState.Listening]
                - referenceModel[(int)EnemyState.Fleeing, (int)EnemyState.Corrupting];
        actualModel[(int)EnemyState.Fleeing, (int)EnemyState.Moving] = max - activePercentageFleeing;
        actualModel[(int)EnemyState.Fleeing, (int)EnemyState.Fleeing] = activePercentageFleeing;

        float activePercentageListening = mapRange(
            activePercentage,
            0, 1,
            0, 1
                - referenceModel[(int)EnemyState.Listening, (int)EnemyState.Listening]
                - referenceModel[(int)EnemyState.Listening, (int)EnemyState.Corrupting]
        );
        max = 1
                - referenceModel[(int)EnemyState.Listening, (int)EnemyState.Listening]
                - referenceModel[(int)EnemyState.Listening, (int)EnemyState.Corrupting];
        actualModel[(int)EnemyState.Listening, (int)EnemyState.Moving] = max - activePercentageListening;
        actualModel[(int)EnemyState.Listening, (int)EnemyState.Fleeing] = activePercentageListening;
    }

    // For Debugging purposes
    void DebugPrint2DArr(float[,] arr)
    {
        String s = "";
        for (int i = 0; i < arr.GetLength(0); i++)
        {
            String a = "";
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                a += " " + arr[i, j];
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
    void SwitchStates()
    {
        // float maxPercent = -1;
        // int currentState = (int)state;
        // for(int i = 0; i < 4; i++) {
        //     float percentCompare = actualModel[currentState,i];
        //     if(percentCompare>maxPercent) {
        //         maxPercent = percentCompare;
        //         state = (EnemyState)i;
        //     }
        // }

        // Essentially, the AI might chose to do something rare, like corrupting, sometimes, especially if it isn't sure.
        // This works I think, becuase random.range should have equal chance of giving a value in the range.
        // This means that if moving and fleeing are both 30% chance, the range should have a 30% chance of giving a value between 0 to .3 and a 30% chance from .3 to .6
        // At least in theory.
        float randGen = Random.Range(0.00f, 1.00f);
        // DebugPrint2DArr(actualModel);
        EnemyState newState = EnemyState.Corrupting;
        if (randGen >= actualModel[(int)state, (int)EnemyState.Corrupting])
        {
            newState = EnemyState.Moving;
        }
        if (randGen >= actualModel[(int)state, (int)EnemyState.Corrupting] + actualModel[(int)state, (int)EnemyState.Moving])
        {
            newState = EnemyState.Fleeing;
        }
        if (randGen >= actualModel[(int)state, (int)EnemyState.Corrupting] + actualModel[(int)state, (int)EnemyState.Moving] + actualModel[(int)state, (int)EnemyState.Fleeing])
        {
            newState = EnemyState.Listening;
        }
        state = newState;
    }

    void UpdatePlayerStats(GameObject body, ref PlayerStats stats)
    {
        float distance = Vector3.Distance(body.transform.position, transform.position);

        // Can you hear?
        if (distance < HearingDistance)
            stats.visibility = PlayerVisibility.CanHear;
        else
            stats.visibility = PlayerVisibility.NotVisible;

        // Can you see?
        if (distance < SightDistance)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, (body.transform.position - transform.position).normalized, out hit, SightDistance) && hit.collider.CompareTag("PlayerBody"))
            {
                stats.visibility = PlayerVisibility.InSight;
            }
        }

        if (stats.visibility == PlayerVisibility.NotVisible)
        {
            // This is so the penalty for being caught isn't applied every frame
            appliedPenalty = false;
            return;
        }

        if (stats.visibility == PlayerVisibility.InSight)
        {
            //Always track when in sight
            stats.MovementDelta = Vector3.Distance(stats.oldPos, body.transform.position);
            stats.oldPos = body.transform.position;
            if (stats.MovementDelta > 0)
            {
                stats.PercentInactive = 1.00f;
                if (!appliedPenalty && distance < HearingDistance)
                {
                    eagerness -= CaughtPenalty;
                    appliedPenalty = true;
                }
            }
            else
            {
                // Decays over time
                stats.PercentInactive -= eagerness * Time.deltaTime / 10f;
                stats.PercentInactive = Mathf.Clamp(stats.PercentInactive, 0f, 1f);
            }
        }
        else if (state == EnemyState.Listening && stats.visibility != PlayerVisibility.NotVisible)
        {
            bool failedHearing = stats.visibility == PlayerVisibility.CanHear && Random.Range(0f, 1f) < MisHearingChance;
            if (!failedHearing)
            {
                stats.MovementDelta = Vector3.Distance(stats.oldPos, body.transform.position);
                stats.oldPos = body.transform.position;
                if (stats.MovementDelta > 0)
                {
                    stats.PercentInactive += eagerness * Time.deltaTime;
                    stats.PercentInactive = Mathf.Clamp(stats.PercentInactive, 0f, 1f);
                }
                else
                {
                    stats.PercentInactive -= eagerness * Time.deltaTime;
                    stats.PercentInactive = Mathf.Clamp(stats.PercentInactive, 0f, 1f);
                }
            }
            else
            {
                stats.MovementDelta = 0;
            }
        }
    }
}

struct PlayerStats
{
    public Vector3 oldPos;
    public float MovementDelta;
    public float PercentInactive;
    public PlayerVisibility visibility;
}

enum PlayerVisibility
{
    InSight, CanHear, NotVisible
}

enum EnemyState
{
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