using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManagerScript : MonoBehaviour
{
    public List<AudioSource> audioSources = new List<AudioSource>();

    public void PlaySound(string clip)
    {
        switch (clip)
        {
            case "launched":
                audioSources[3].PlayOneShot(audioSources[3].clip);
                break;
            case "hitTarget":
                audioSources[2].PlayOneShot(audioSources[2].clip);
                break;
            case "missedTarget":
                audioSources[4].PlayOneShot(audioSources[4].clip);
                break;
            case "startGameMusic":
                audioSources[5].PlayOneShot(audioSources[5].clip);
                break;
            case "gameOver":
                audioSources[1].PlayOneShot(audioSources[1].clip);
                break;
            case "buttonPress":
                audioSources[0].PlayOneShot(audioSources[0].clip);
                break;
        }
    }
}
