using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{

    [SerializeField]
    private AudioSource BGM;

    [SerializeField]
    private AudioClip[] sounds;
    private AudioSource sfx;

    void Start() {
        sfx = GetComponent<AudioSource>();
    }

    public void PlaySound(int soundInd) {
        sfx.PlayOneShot(sounds[soundInd]);
    }

    public void StopBGM() {
        BGM.Stop();
    }
}
