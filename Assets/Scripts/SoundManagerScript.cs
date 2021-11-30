using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public static AudioClip bugDeathSound, bugRmbSound, glitchDeathSound, glitchInitSound, chocolateSound, coffeeSound, autoTestSound, levelFinishSound, gameOverSound, level1Sound, level2Sound, level3Sound, level4Sound, menuSound, titleSound;
    static AudioSource audioSrc;

    // Start is called before the first frame update
    void Start()
    {
        bugDeathSound = Resources.Load<AudioClip>("SFX_BugDeath");
        bugRmbSound = Resources.Load<AudioClip>("SFX_BugRmb");
        glitchDeathSound = Resources.Load<AudioClip>("SFX_GlitchDeath");
        glitchInitSound = Resources.Load<AudioClip>("SFX_GlitchInit");
        chocolateSound = Resources.Load<AudioClip>("SFX_UseChocolate");
        coffeeSound = Resources.Load<AudioClip>("SFX_UseCoffee");
        autoTestSound = Resources.Load<AudioClip>("SFX_UseAutoTest");
        levelFinishSound = Resources.Load<AudioClip>("SFX_LevelFinishSuccess");
        gameOverSound = Resources.Load<AudioClip>("SFX_LevelGameOver");

        level1Sound = Resources.Load<AudioClip>("BGM_Level1");
        level2Sound = Resources.Load<AudioClip>("BGM_Level2");
        level3Sound = Resources.Load<AudioClip>("BGM_Level3");
        level4Sound = Resources.Load<AudioClip>("BGM_Level4");
        menuSound = Resources.Load<AudioClip>("BGM_Menu");
        titleSound = Resources.Load<AudioClip>("BGM_Titles");

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
                audioSrc.PlayOneShot(glitchDeathSound);
                break;
            case "GlitchInit":
                audioSrc.PlayOneShot(glitchInitSound);
                break;         
            case "UseChocolate":
                audioSrc.PlayOneShot(chocolateSound);
                break;
            case "UseCoffee":
                audioSrc.PlayOneShot(coffeeSound);
                break;
            case "UseAutoTest":
                audioSrc.PlayOneShot(autoTestSound);
                break;
            case "LevelSuccess":
                audioSrc.PlayOneShot(levelFinishSound);
                break;
            case "GameOver":
                audioSrc.PlayOneShot(gameOverSound);
                break;
            case "Level_1":
                audioSrc.PlayOneShot(level1Sound);
                break;
            case "Level_2":
                audioSrc.PlayOneShot(level2Sound);
                break;
            case "Level_3":
                audioSrc.PlayOneShot(level3Sound);
                break;
            case "Level_4":
                audioSrc.PlayOneShot(level4Sound);
                break;
            case "Menu":
                audioSrc.PlayOneShot(menuSound);
                break;
            case "Titles":
                audioSrc.PlayOneShot(titleSound);
                break;
        }
    }
}
