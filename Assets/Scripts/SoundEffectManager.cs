using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager Instance;
    public AudioSource myAudio;
    public List<AudioClip> soundEffects;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance != null)
        {
            return;
        }
        resetSoundEffect();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void playAudio(int audioIndex)
    {
        myAudio.clip = soundEffects[audioIndex];
        myAudio.Play();
    }

    void resetSoundEffect()
    {
        Instance = this;
    }
}