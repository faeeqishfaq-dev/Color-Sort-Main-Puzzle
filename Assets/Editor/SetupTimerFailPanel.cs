using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class SetupTimerFailPanel : EditorWindow
{
    [MenuItem("Tools/Setup Timer UI (All Levels)")]
    public static void SetupAllLevels()
    {
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes/Levels" });
        
        foreach (string guid in sceneGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            
            SetupPanelInCurrentScene();
            
            EditorSceneManager.SaveScene(scene);
        }
        
        Debug.Log("Successfully added Timer UI (Panel and Text) to all level scenes! You can now open any scene and edit them.");
    }
    
    private static void SetupPanelInCurrentScene()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;
        
        // Remove existing if any
        Transform existing = canvas.transform.Find("TimerFailPanel");
        if (existing != null)
        {
            DestroyImmediate(existing.gameObject);
        }
        
        // Find LevelCompleted
        Transform winPanel = canvas.transform.Find("LevelCompleted");
        if (winPanel == null) return;
        
        // Clone it
        GameObject failPanel = Instantiate(winPanel.gameObject, canvas.transform);
        failPanel.name = "TimerFailPanel";
        failPanel.transform.SetAsLastSibling();
        
        // Remove LevelCompleted component so it doesn't trigger win logic/sounds
        Component lcComp = failPanel.GetComponent("LevelCompleted");
        if (lcComp != null) DestroyImmediate(lcComp);
        
        // Change Texts to 'You lose'
        TextMeshProUGUI[] tmps = failPanel.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var t in tmps)
        {
            if (t.text.ToLower().Contains("won") || t.text.ToLower().Contains("win"))
                t.text = "You lose";
        }
        Text[] txts = failPanel.GetComponentsInChildren<Text>(true);
        foreach (var t in txts)
        {
            if (t.text.ToLower().Contains("won") || t.text.ToLower().Contains("win"))
                t.text = "You lose";
        }
        
        // Change Green Panel to Red Panel
        Sprite redSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/red_panel.png");
        if (redSprite != null)
        {
            Image[] images = failPanel.GetComponentsInChildren<Image>(true);
            foreach (var img in images)
            {
                if (img.sprite != null && img.sprite.name.ToLower().Contains("green"))
                {
                    img.sprite = redSprite;
                }
            }
        }
        
        failPanel.SetActive(false);

        // --- Setup LevelTimerText ---
        Transform textExisting = canvas.transform.Find("LevelTimerText");
        if (textExisting != null)
        {
            DestroyImmediate(textExisting.gameObject);
        }
        
        GameObject textObj = new GameObject("LevelTimerText");
        textObj.transform.SetParent(canvas.transform, false);
        textObj.transform.SetAsLastSibling(); // Ensure it renders on top
        
        TextMeshProUGUI timerText = textObj.AddComponent<TextMeshProUGUI>();
        
        // Find project font if possible
        TMP_FontAsset projectFont = null;
        TextMeshProUGUI[] existingTexts = canvas.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var txt in existingTexts)
        {
            if (txt.font != null)
            {
                projectFont = txt.font;
                break;
            }
        }
        if (projectFont != null) timerText.font = projectFont;
        
        timerText.alignment = TextAlignmentOptions.TopRight;
        timerText.fontSize = 70;
        timerText.color = Color.white;
        timerText.fontStyle = FontStyles.Bold;
        timerText.text = "00:00";
        
        RectTransform rect = timerText.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(1, 1);
        rect.anchoredPosition = new Vector2(-50, -50); 
        rect.sizeDelta = new Vector2(400, 100);
    }
}
