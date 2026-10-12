using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject startMenuUI;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject settingsWindow;
    [SerializeField] private GameObject playButton; // Assign Play Button here

    [Header("Loading Bar")]
    [SerializeField] private Image loadingFill;

    [Header("Loading Settings")]
    [SerializeField] private float loadingDuration = 5f;
    [SerializeField] private string levelsMenuSceneName = "LevelsMenu";

    private bool isLoading;

    private void Awake()
    {
        loadingPanel.SetActive(true);
        startMenuUI.SetActive(false);

        if (settingsWindow != null)
            settingsWindow.SetActive(false);

        loadingFill.fillAmount = 0f;

        isLoading = true;
    }

    private void Start()
    {
        StartCoroutine(InitialLoading());
    }

    private IEnumerator InitialLoading()
    {
        //CheckCpAd.instance.CallAd();

        float timer = 0f;
        loadingFill.fillAmount = 0f;

        while (timer < loadingDuration)
        {
            timer += Time.unscaledDeltaTime;

            loadingFill.fillAmount =
                Mathf.Clamp01(timer / loadingDuration);

            yield return null;
        }

        loadingFill.fillAmount = 1f;

        loadingPanel.SetActive(false);
        startMenuUI.SetActive(true);

        isLoading = false;
    }

    public void PlayButton()
    {
        if (isLoading)
            return;

        StartCoroutine(LoadLevelsMenu());
    }

    private IEnumerator LoadLevelsMenu()
    {
        isLoading = true;

        startMenuUI.SetActive(false);
        loadingPanel.SetActive(true);

        loadingFill.fillAmount = 0f;
        CheckCpAd.instance.CallAd();
        AsyncOperation operation =
            SceneManager.LoadSceneAsync(levelsMenuSceneName);

        operation.allowSceneActivation = false;

        float timer = 0f;

        while (timer < loadingDuration || operation.progress < 0.9f)
        {
            timer += Time.unscaledDeltaTime;

            float timeProgress =
                Mathf.Clamp01(timer / loadingDuration);

            float sceneProgress =
                Mathf.Clamp01(operation.progress / 0.9f);

            loadingFill.fillAmount =
                Mathf.Min(timeProgress, sceneProgress);

            yield return null;
        }

        loadingFill.fillAmount = 1f;

        operation.allowSceneActivation = true;
    }

    public void ShowSettings()
    {
        startMenuUI.SetActive(false);
        settingsWindow.SetActive(true);
    }

    public void ExitPanel()
    {
        settingsWindow.SetActive(false);
        startMenuUI.SetActive(true);
    }
}