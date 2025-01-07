using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;

    public AudioClip background;
    public AudioClip gameBackground;
    public static AudioManager instance;
    TypeMusic type = TypeMusic.UIMusic;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }
    
    public void ChangeClip(TypeMusic incoming) {
        if(type == incoming)
            return;
        type = incoming;
        musicSource.Stop();
        switch (type) {
            case TypeMusic.GameBG:
                musicSource.clip = gameBackground;
                musicSource.Play();
                break;
            case TypeMusic.UIMusic:
                musicSource.clip = background;
                musicSource.Play();
                break;
            default:
                break;
        }
    }
}

public enum TypeMusic {
    UIMusic, GameBG, None
}