using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public static AudioClip bugDeathSound, bugRmbSound, glitchDeath, glitchInit, autoTest;
    static AudioSource audioSrc;

    // Start is called before the first frame update
    void Start()
    {
        bugDeathSound = Resources.Load<AudioClip>("SFX_BugDeath");
        bugRmbSound = Resources.Load<AudioClip>("SFX_BugRmb");
        glitchDeath = Resources.Load<AudioClip>("SFX_GlitchDeath");
        glitchInit = Resources.Load<AudioClip>("SFX_GlitchInit");
        autoTest = Resources.Load<AudioClip>("SFX_UseAutoTest");
    
        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "BugDeath":
                audioSrc.PlayOneShot(bugDeathSound);
                break;
            case "BugRmb":
                audioSrc.PlayOneShot(bugRmbSound);
                break;
            case "GlitchDeath":
                audioSrc.PlayOneShot(glitchDeath);
                break;
            case "GlitchInit":
                audioSrc.PlayOneShot(glitchInit);
                break;
            case "AutoTest":
                audioSrc.PlayOneShot(autoTest);
                break;
        }
    }
}
