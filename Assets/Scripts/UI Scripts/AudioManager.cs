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
    public AudioClip playerShoot; //shoot
    public AudioClip playerSwitch; //done
    public AudioClip lose; //done
    public AudioClip enemyDie; //done
    public AudioClip shocked; //worksdone


     public void PlaySFX(AudioClip clip)
    {
        SFX.clip = clip;
        SFX.Play();
    }

    public void StopSFX()
    {
        if (SFX.isPlaying)
        {
            SFX.Stop();
        }
    }

     public bool IsSFXPlaying()
    {
        return SFX.isPlaying;
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

  /* Keep this commented
    AudioManager audioManager;
    private void Awake(){
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    audioManager.PlaySFX(audioManager.lose);
  */

