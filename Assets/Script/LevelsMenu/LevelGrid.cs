using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelGrid : MonoBehaviour
{
    [Header("Level Buttons")]
    [SerializeField] private Button[] button;

    [Range(0, 15)]
    [SerializeField] private int unlockedLevel;

    [Header("Loading UI")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Image loadingFill;

    [Header("Loading Settings")]
    [SerializeField] private float loadingDuration = 5f;

    private bool isLoading = false;

    private void Start()
    {
        // First level unlock
        if (!PlayerPrefs.HasKey("LevelIsUnlocked"))
        {
            PlayerPrefs.SetInt("LevelIsUnlocked", 1);
            PlayerPrefs.Save();
        }

        // Loading panel start mein hidden
        if (loadingPanel != null)
            loadingPanel.SetActive(false);

        if (loadingFill != null)
            loadingFill.fillAmount = 0f;

        UpdateLevelButtons();
    }

    private void Update()
    {
        UpdateLevelButtons();
    }

    private void UpdateLevelButtons()
    {
        unlockedLevel = PlayerPrefs.GetInt("LevelIsUnlocked", 1);

        // Pehle sab buttons lock
        for (int i = 0; i < button.Length; i++)
        {
            button[i].interactable = false;
        }

        // Unlocked levels enable
        int count = Mathf.Min(unlockedLevel, button.Length);

        for (int i = 0; i < count; i++)
        {
            button[i].interactable = true;
        }
    }

    // LevelScene script is function ko call karegi
    public void LoadLevel(int sceneIndex)
    {
        if (isLoading)
            return;

        if (loadingPanel == null || loadingFill == null)
        {
            Debug.LogError("Loading Panel ya Loading Fill assign nahi hai!");
            return;
        }

        if (sceneIndex < 0 ||
            sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError("Invalid Scene Index: " + sceneIndex);
            return;
        }
        //CheckCpAd.instance.CallAd();

        StartCoroutine(LoadLevelRoutine(sceneIndex));
    }

    private IEnumerator LoadLevelRoutine(int sceneIndex)
    {
        isLoading = true;

        // Pehle Ad show karo
        if (CheckCpAd.instance != null)
        {
            CheckCpAd.instance.CallAd();

            // Agar Ad open hui hai to close hone ka wait karo
            if (CheckCpAd.instance.cpVideoAd != null &&
                CheckCpAd.instance.cpVideoAd.activeSelf)
            {
                while (CheckCpAd.instance.cpVideoAd.activeSelf)
                {
                    yield return null;
                }
            }
        }

        // Ad close hone ke BAAD loading screen show
        loadingPanel.SetActive(true);

        loadingFill.fillAmount = 0f;

        AsyncOperation operation =
            SceneManager.LoadSceneAsync(sceneIndex);

        operation.allowSceneActivation = false;

        float timer = 0f;

        while (timer < loadingDuration ||
               operation.progress < 0.9f)
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

        yield return null;

        operation.allowSceneActivation = true;
    }
}