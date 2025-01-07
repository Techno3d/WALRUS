using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyToken : MonoBehaviour
{
    public Sprite token, tokenDead;
    Image component;
    bool isDead = false;
    void Start() {
        component = GetComponent<Image>();
    }

    public void ChangeToken() {
        isDead = !isDead;
        if(isDead) {
            component.sprite = tokenDead;
        } else {
            component.sprite = token;
        }
    }
}
