using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip Coin, PowerUp, Jump, Crash, Slide, Countdown;

    public static AudioClip CoinSound, PowerUpSound, JumpSound, CrashSound, SlideSound, CountdownSound;
    static AudioSource audioSrc;

    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        JumpSound = Jump;
        CoinSound = Coin;
        PowerUpSound = PowerUp;
        CrashSound = Crash;
        CountdownSound = Countdown;
        SlideSound = Slide;
    }

    public static void PlaySound(string soundClip)
    {
        switch (soundClip)
        {
            case "Coin":
                audioSrc.PlayOneShot(CoinSound);
                break;
            case "PowerUp":
                audioSrc.PlayOneShot(PowerUpSound);
                break;
            case "Jump":
                audioSrc.PlayOneShot(JumpSound);
                break;
            case "Crash":
                audioSrc.PlayOneShot(CrashSound);
                break;
            case "Slide":
                audioSrc.PlayOneShot(SlideSound);
                break;
            case "Countdown":
                audioSrc.PlayOneShot(CountdownSound);
                break;
        }
    }
}
