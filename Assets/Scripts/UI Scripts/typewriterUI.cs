// Script for having a typewriter effect for UI
// Prepared by Nick Hwang (https://www.youtube.com/nickhwang)
// Want to get creative? Try a Unicode leading character(https://unicode-table.com/en/blocks/block-elements/)
// Copy Paste from page into Inpector

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class typewriterUI : MonoBehaviour
{
    Text _text;
    TMP_Text _tmpProText;
    string writer;

    [SerializeField] float delayBeforeStart = 0f;
    [SerializeField] float timeBtwChars = 0.1f;
    [SerializeField] string leadingChar = "";
    [SerializeField] bool leadingCharBeforeDelay = false;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private AudioClip audioClip2;

    // Reference to the AudioSource component
    private AudioSource audioSource;
    private AudioSource audioSource2;


    // Use this for initialization
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource2 = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource2.clip = audioClip2;
        audioSource.loop = true;
        audioSource2.loop = true;
        _text = GetComponent<Text>()!;
        _tmpProText = GetComponent<TMP_Text>()!;

        if (_text != null)
        {
            writer = _text.text;
            _text.text = "";

            StartCoroutine("TypeWriterText");
        }

        if (_tmpProText != null)
        {
            writer = _tmpProText.text;
            _tmpProText.text = "";

            StartCoroutine("TypeWriterTMP");
        }
    }

    IEnumerator TypeWriterText()
    {
        audioSource.Play();
        _text.text = leadingCharBeforeDelay ? leadingChar : "";

        yield return new WaitForSeconds(delayBeforeStart);

        foreach (char c in writer)
        {
            if (_text.text.Length > 0)
            {
                _text.text = _text.text.Substring(0, _text.text.Length - leadingChar.Length);
            }
            _text.text += c;
            _text.text += leadingChar;
            yield return new WaitForSeconds(timeBtwChars);
        }

        if (leadingChar != "")
        {
            _text.text = _text.text.Substring(0, _text.text.Length - leadingChar.Length);
        }
        audioSource.Stop();
    }

    IEnumerator TypeWriterTMP()
    {
        audioSource.Play();
        _tmpProText.text = leadingCharBeforeDelay ? leadingChar : "";
        int count = 0;
        yield return new WaitForSeconds(delayBeforeStart);
        bool hasdone = false;
        foreach (char c in writer)
        {
            if (_tmpProText.text.Length > 0)
            {

                _tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
            }
            _tmpProText.text += c;
            _tmpProText.text += leadingChar;
            if (count > 4)
            {
                Debug.Log(count);
                if (_tmpProText.text.Contains("...") && hasdone == false)
                {
                    audioSource.Stop();
                    audioSource2.Play();
                    hasdone = true;
                }
            }
            count++;
            yield return new WaitForSeconds(timeBtwChars);
        }

        if (leadingChar != "")
        {
            _tmpProText.text = _tmpProText.text.Substring(0, _tmpProText.text.Length - leadingChar.Length);
        }
        audioSource.Stop();
    }
    private void OnDestroy()
    {
        if (audioSource && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        if (audioSource2 && audioSource2.isPlaying)
        {
            audioSource2.Stop();
        }
    }
}