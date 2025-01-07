using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterTracker : MonoBehaviour
{
    public GameObject tokenPrefab;
    GameObject[] tokens;
    int numDead = 0;
    bool set = false;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(!set) {
            Debug.Log(Enemy.TotalNumEnemies);
            tokens = new GameObject[Enemy.TotalNumEnemies];
            for(int i = 0; i < tokens.Length; i++) {
                tokens[i] = Instantiate(tokenPrefab, transform, false);
            }
            EnemyHealth.EnemyDeath += () => {
                numDead++;
                if(numDead>tokens.Length) {
                    return;
                }
                tokens[numDead-1].GetComponent<EnemyToken>().ChangeToken();
            };
            set = true;
        }
    }
}
