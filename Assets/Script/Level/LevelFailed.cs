using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFailed : MonoBehaviour
{
    private GameObject levelFailPanel;
    private CanvasGroup failCanvasGroup;

    private GameController gameController;

    private int sceneIndex;

    private bool isFailed = false;
    private bool isChangingScene = false;

    private void Awake()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex;

        gameController = GetComponent<GameController>();

        if (gameController == null)
            gameController = FindObjectOfType<GameController>();
    }

    // LevelTimer automatically panel yahan assign karega
    public void SetFailPanel(GameObject panel)
    {
        levelFailPanel = panel;

        if (levelFailPanel == null)
            return;

        failCanvasGroup =
            levelFailPanel.GetComponent<CanvasGroup>();

        if (failCanvasGroup == null)
        {
            failCanvasGroup =
                levelFailPanel.AddComponent<CanvasGroup>();
        }

        failCanvasGroup.interactable = true;
        failCanvasGroup.blocksRaycasts = true;

        levelFailPanel.SetActive(false);
    }

    // Time 0 hone par LevelTimer isko call karega
    public void ShowFail()
    {
        if (isFailed)
            return;

        isFailed = true;

        // Bottle gameplay stop
        if (gameController != null)
        {
            gameController.enabled = false;
        }

        if (levelFailPanel != null)
        {
            levelFailPanel.SetActive(true);

            if (failCanvasGroup != null)
            {
                failCanvasGroup.interactable = true;
                failCanvasGroup.blocksRaycasts = true;
            }
        }
        else
        {
            Debug.LogError("TimerFailPanel not found!");
        }
    }

    // REPLAY
    public void Replay()
    {
        if (isChangingScene)
            return;

        StartCoroutine(
            ShowAdThenLoad(sceneIndex)
        );
    }

    // HOME
    public void Home()
    {
        if (isChangingScene)
            return;

        StartCoroutine(
            ShowAdThenLoad(0)
        );
    }

    private IEnumerator ShowAdThenLoad(int targetScene)
    {
        isChangingScene = true;

        // Fail panel ad ke Cross button ko block na kare
        if (failCanvasGroup != null)
        {
            failCanvasGroup.interactable = false;
            failCanvasGroup.blocksRaycasts = false;
        }

        // Ad show
        if (CheckCpAd.instance != null)
        {
            CheckCpAd.instance.CallAd();

            // Agar ad actually show hui hai
            // to cross se close hone ka wait
            if (CheckCpAd.instance.cpVideoAd != null &&
                CheckCpAd.instance.cpVideoAd.activeSelf)
            {
                while (
                    CheckCpAd.instance.cpVideoAd.activeSelf
                )
                {
                    yield return null;
                }
            }
        }

        // Safety
        Time.timeScale = 1f;

        // Replay par same scene reload hogi,
        // isliye timer bhi fresh start hoga
        SceneManager.LoadScene(targetScene);
    }
}