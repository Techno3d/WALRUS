using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("---- Some Stuff ----")]
    [SerializeField] AudioSource musicSource;

    public AudioClip background;
    public AudioClip gameBackground;
    public static AudioManager instance;
    TypeMusic type = TypeMusic.UIMusic;

    [Header("---- The Source ----")]
    [SerializeField] AudioSource SFX;

    [Header("---- The Effects ----")] 
    public AudioClip playerShoot; 
    public AudioClip playerSwitch; 
    public AudioClip win; 
    public AudioClip lose; //done
    public AudioClip enemyDie; 
    public AudioClip shocked; //done


    public void PlaySFX(AudioClip clip){
        SFX.PlayOneShot(clip);
    }

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

  

