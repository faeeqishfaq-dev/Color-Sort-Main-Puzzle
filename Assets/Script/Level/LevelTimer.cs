using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    private float timeRemaining;
    private bool isTimerRunning = false;
    private int levelNumber;

    private TextMeshProUGUI timerText;

    private GameObject failPanel;

    private GameController gameController;
    private PauseGame pauseGame;
    private LevelFailed levelFailed;

    private void Start()
    {
        gameController =
            FindObjectOfType<GameController>();

        pauseGame =
            FindObjectOfType<PauseGame>();

        // LevelFailed same GameController par hogi
        levelFailed =
            GetComponent<LevelFailed>();

        if (levelFailed == null &&
            gameController != null)
        {
            levelFailed =
                gameController.GetComponent<LevelFailed>();
        }

        if (levelFailed == null)
        {
            levelFailed =
                FindObjectOfType<LevelFailed>();
        }

        DetermineLevelNumber();

        // Level 1 = 30 sec
        // Level 2 = 60 sec
        // Level 3 = 90 sec...
        timeRemaining =
            levelNumber * 30f;

        CreateUI();

        UpdateTimerText();

        isTimerRunning = true;
    }

    private void DetermineLevelNumber()
    {
        string sceneName =
            SceneManager.GetActiveScene().name;

        if (sceneName.StartsWith("Level "))
        {
            if (
                int.TryParse(
                    sceneName.Substring(6),
                    out int num
                )
            )
            {
                levelNumber = num;
                return;
            }
        }

        // Safe fallback
        levelNumber =
            Mathf.Max(
                1,
                SceneManager
                    .GetActiveScene()
                    .buildIndex - 1
            );
    }

    private void Update()
    {
        if (!isTimerRunning)
            return;

        // Level complete ho gaya
        if (
            gameController != null &&
            gameController.LevelCompleted != null &&
            gameController.LevelCompleted.activeSelf
        )
        {
            isTimerRunning = false;
            return;
        }

        // Pause Menu
        if (
            pauseGame != null &&
            pauseGame.pauseMenu != null &&
            pauseGame.pauseMenu.activeSelf
        )
        {
            return;
        }

        // Game paused
        if (Time.timeScale == 0f)
            return;

        // Timer
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;

            UpdateTimerText();

            TriggerLevelFailed();

            return;
        }

        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        if (timerText == null)
            return;

        int minutes =
            Mathf.FloorToInt(
                timeRemaining / 60f
            );

        int seconds =
            Mathf.FloorToInt(
                timeRemaining % 60f
            );

        timerText.text =
            string.Format(
                "{0:00}:{1:00}",
                minutes,
                seconds
            );
    }

    private void TriggerLevelFailed()
    {
        if (!isTimerRunning)
            return;

        // Timer completely stop
        isTimerRunning = false;

        // Separate LevelFailed system
        if (levelFailed != null)
        {
            levelFailed.ShowFail();
        }
        else
        {
            Debug.LogError(
                "LevelFailed script not found!"
            );

            // Emergency fallback
            if (failPanel != null)
                failPanel.SetActive(true);

            if (gameController != null)
                gameController.enabled = false;
        }
    }

    private void CreateUI()
    {
        Canvas canvas =
            FindObjectOfType<Canvas>();

        if (canvas == null)
        {
            Debug.LogWarning(
                "LevelTimer: No Canvas found."
            );

            return;
        }

        // =========================
        // TIMER TEXT
        // =========================

        TextMeshProUGUI[] existingTexts =
            Resources
                .FindObjectsOfTypeAll<TextMeshProUGUI>();

        TMP_FontAsset projectFont = null;

        if (
            existingTexts != null &&
            existingTexts.Length > 0
        )
        {
            foreach (
                TextMeshProUGUI txt
                in existingTexts
            )
            {
                if (txt.font != null)
                {
                    projectFont = txt.font;
                    break;
                }
            }
        }

        Transform textTransform =
            canvas.transform.Find(
                "LevelTimerText"
            );

        if (textTransform != null)
        {
            timerText =
                textTransform
                    .GetComponent<TextMeshProUGUI>();
        }
        else
        {
            GameObject textObj =
                new GameObject(
                    "LevelTimerText"
                );

            textObj.transform.SetParent(
                canvas.transform,
                false
            );

            textObj.transform
                .SetAsLastSibling();

            timerText =
                textObj
                    .AddComponent<TextMeshProUGUI>();

            if (projectFont != null)
                timerText.font = projectFont;

            timerText.alignment =
                TextAlignmentOptions.TopRight;

            timerText.fontSize = 70;

            timerText.color =
                Color.white;

            timerText.fontStyle =
                FontStyles.Bold;

            RectTransform rect =
                timerText
                    .GetComponent<RectTransform>();

            rect.anchorMin =
                new Vector2(1, 1);

            rect.anchorMax =
                new Vector2(1, 1);

            rect.pivot =
                new Vector2(1, 1);

            rect.anchoredPosition =
                new Vector2(-50, -50);

            rect.sizeDelta =
                new Vector2(400, 100);
        }

        // =========================
        // FAIL PANEL
        // =========================

        Transform fpTransform =
            canvas.transform.Find(
                "TimerFailPanel"
            );

        if (fpTransform != null)
        {
            failPanel =
                fpTransform.gameObject;

            failPanel.SetActive(false);

            // Panel LevelFailed ko de do
            if (levelFailed != null)
            {
                levelFailed.SetFailPanel(
                    failPanel
                );
            }

            // =====================
            // BUTTONS
            // =====================

            Button[] buttons =
                failPanel
                    .GetComponentsInChildren<Button>(
                        true
                    );

            foreach (Button btn in buttons)
            {
                string btnName =
                    btn.gameObject
                        .name
                        .ToLower();

                // Next button Fail Panel par nahi chahiye
                if (
                    btnName.Contains("next") ||
                    btnName.Contains("forward")
                )
                {
                    btn.gameObject
                        .SetActive(false);
                }

                // REPLAY
                else if (
                    btnName.Contains("play") ||
                    btnName.Contains("again") ||
                    btnName.Contains("restart") ||
                    btnName.Contains("retry")
                )
                {
                    // IMPORTANT:
                    // Purana LevelCompleted.PlayAgain
                    // bhi remove ho jayega
                    btn.onClick =
                        new Button.ButtonClickedEvent();

                    if (levelFailed != null)
                    {
                        btn.onClick.AddListener(
                            levelFailed.Replay
                        );
                    }
                }

                // HOME
                else if (
                    btnName.Contains("home") ||
                    btnName.Contains("menu")
                )
                {
                    btn.onClick =
                        new Button.ButtonClickedEvent();

                    if (levelFailed != null)
                    {
                        btn.onClick.AddListener(
                            levelFailed.Home
                        );
                    }
                }
            }
        }
        else
        {
            Debug.LogError(
                "TimerFailPanel not found under Canvas!"
            );
        }
    }
}