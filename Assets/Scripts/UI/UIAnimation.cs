using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIAnimation : MonoBehaviour
{
    private Animator fadeToBlackTransitionAnimator;

    private void Awake()
    {
        // Get the animator component
        if (GetComponent<Animator>())
        {
            fadeToBlackTransitionAnimator = GetComponent<Animator>();
        }
        else
        {
            Debug.Log("UIAnimation has no animator component!");
        }
    }

    public void TriggerFadeAnimation()
    {
        fadeToBlackTransitionAnimator.SetTrigger("Begin Fade");
        SceneManager.LoadSceneAsync(1);
    }

    public void LoadMainScene()
    {
        MenuManager.instance.LoadMainScene();
    }
}