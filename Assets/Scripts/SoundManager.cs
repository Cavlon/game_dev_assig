using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{

    [SerializeField]
    private AudioSource BGM;

    [SerializeField]
    private AudioClip[] sounds;
    private AudioSource sfx;

    public void PlaySound(int soundInd) {
        if (sfx == null) sfx = GetComponent<AudioSource>();
        sfx.PlayOneShot(sounds[soundInd]);
    }

    public void StopBGM() {
        BGM.Stop();
    }
}
