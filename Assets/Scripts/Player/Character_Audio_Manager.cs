using System;
using UnityEngine;
using UnityEngine.Audio;

public class Character_Audio_Manager : MonoBehaviour
{
    public static Character_Audio_Manager audioInst;

    AudioSource audioSource;

    [SerializeField] AudioClip run;
    [SerializeField] AudioClip jump;
    private void Awake()
    {
        audioInst = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void FootStep()
    {
        StopAudio();
        audioSource.clip = run;
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public void JumpAudio()
    {
        StopAudio();
        audioSource.clip = jump;
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public void StopAudio()
    {
        audioSource.Stop();
    }
}
