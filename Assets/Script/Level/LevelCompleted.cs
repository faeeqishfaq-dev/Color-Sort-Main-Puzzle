using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCompleted : MonoBehaviour
{
    public Button nextLevel;
    public Button playAgain;
    public Button home;

    private int sceneIndex;

    public AudioSource winngSound;

    private void Start()
    {
        PlaySound();
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void NextLevel()
    {
        int targetScene;

        if (sceneIndex == 16)
            targetScene = 0;
        else
            targetScene = sceneIndex + 1;

        StartCoroutine(ShowAdThenLoad(targetScene));
    }

    public void PlayAgain()
    {
        StartCoroutine(ShowAdThenLoad(sceneIndex));
    }

    public void Home()
    {
        StartCoroutine(ShowAdThenLoad(0));
    }

    private IEnumerator ShowAdThenLoad(int targetScene)
    {
        // Fail/Complete panel ko visible rehne do,
        // lekin ad ke clicks block na karne do.
        DisableCurrentPanelInput();

        if (CheckCpAd.instance != null)
        {
            CheckCpAd.instance.CallAd();

            if (CheckCpAd.instance.cpVideoAd != null &&
                CheckCpAd.instance.cpVideoAd.activeSelf)
            {
                while (CheckCpAd.instance.cpVideoAd.activeSelf)
                {
                    yield return null;
                }
            }
        }

        SceneManager.LoadScene(targetScene);
    }

    private void DisableCurrentPanelInput()
    {
        // Screenshot ke mutabiq script PlayAgain button par lagi hui hai
        // aur uska parent TimerFailPanel / LevelCompleted panel hai.
        if (transform.parent == null)
            return;

        CanvasGroup canvasGroup =
            transform.parent.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = transform.parent.gameObject.AddComponent<CanvasGroup>();

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void PlaySound()
    {
        winngSound.Play();
    }
}