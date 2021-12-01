using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMng : MonoBehaviour
{
    [SerializeField] private string currScene;
    [SerializeField] private string nextScene;
    public void GoToNextLevel()
    {
        SceneLoader.SwitchToScene(nextScene);
    }

    public void RestartLevel() 
    {
        SceneLoader.SwitchToScene(currScene);
    }
}
