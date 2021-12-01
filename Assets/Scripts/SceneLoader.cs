using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance;


    private Animator loaderAnimator;
    private AsyncOperation loaingSceneOperation;
    public static void SwitchToScene(string sceneName)
    {
        instance.loaderAnimator.SetTrigger("Start");

        instance.loaingSceneOperation = SceneManager.LoadSceneAsync(sceneName);
        instance.loaingSceneOperation.allowSceneActivation = false;
    }

    private void Start()
    {
        instance = this;

        loaderAnimator = GetComponent<Animator>();
    }

    public void OnLoadingOver()    
    {
        loaingSceneOperation.allowSceneActivation = true;
    }
}
