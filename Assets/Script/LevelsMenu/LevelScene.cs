using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelScene : MonoBehaviour
{
    public int sceneIndex;

    private LevelGrid levelGrid;

    private void Awake()
    {
        // Ye button Grid ka child hai,
        // isliye parent se LevelGrid automatically mil jayega
        levelGrid = GetComponentInParent<LevelGrid>();
    }

    public void OpenScene()
    {
        if (levelGrid != null)
        {
            levelGrid.LoadLevel(sceneIndex);
        }
        else
        {
            Debug.LogError(
                "LevelGrid nahi mila: " + gameObject.name
            );
        }
    }

    public void BackToMenu()
    {
        CheckCpAd.instance.CallAd();
        SceneManager.LoadScene(1);
    }
}