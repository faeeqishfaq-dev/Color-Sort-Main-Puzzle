using System.Collections;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public BottleController FirstBottle;
    public BottleController SecondBottle;
    public BottleController[] bottles;

    private bool allFull = false;
    private bool levelFailed = false;

    public int levelToUnlock;
    private int numberOfUnlockedLevel;

    public GameObject LevelCompleted;

    private float bottleUp = 0.3f;
    private float bottleDown = -0.3f;

    private void Start()
    {
        if (GetComponent<LevelFailed>() == null)
        {
            gameObject.AddComponent<LevelFailed>();
        }

        if (GetComponent<LevelTimer>() == null)
        {
            gameObject.AddComponent<LevelTimer>();
        }
    }

    private void Update()
    {
        // Level fail hone ke baad gameplay band
        if (levelFailed)
            return;

        HandleBottleInput();
        CheckLevelCompletion();
    }

    // LevelFailed script isko call karegi
    public void SetLevelFailed()
    {
        // Agar level already complete ho gaya hai
        // to fail nahi hona chahiye
        if (allFull)
            return;

        levelFailed = true;

        DeselectBottle();
    }

    private void HandleBottleInput()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError(
                "Main Camera was not found. Make sure your camera has the MainCamera tag."
            );

            return;
        }

        Vector3 mousePosition =
            mainCamera.ScreenToWorldPoint(Input.mousePosition);

        Vector2 mousePosition2D =
            new Vector2(mousePosition.x, mousePosition.y);

        RaycastHit2D hit =
            Physics2D.Raycast(mousePosition2D, Vector2.zero);

        BottleController clickedBottle = null;

        if (hit.collider != null)
        {
            clickedBottle =
                hit.collider.GetComponent<BottleController>();
        }

        if (clickedBottle == null)
        {
            DeselectBottle();
            return;
        }

        if (FirstBottle == null)
        {
            if (clickedBottle.numberOfColorsInBottle == 0)
                return;

            FirstBottle = clickedBottle;

            MoveBottle(FirstBottle, bottleUp);

            return;
        }

        if (FirstBottle == clickedBottle)
        {
            DeselectBottle();
            return;
        }

        SecondBottle = clickedBottle;

        FirstBottle.bottleControllerRef = SecondBottle;

        FirstBottle.UpdateTopColorValue();
        SecondBottle.UpdateTopColorValue();

        bool canTransfer =
            SecondBottle.FillBottleCheck(
                FirstBottle.topColor
            );

        if (canTransfer)
        {
            FirstBottle.startColorTransfer();

            FirstBottle = null;
            SecondBottle = null;
        }
        else
        {
            DeselectBottle();
        }
    }

    private void DeselectBottle()
    {
        if (FirstBottle != null)
        {
            if (FirstBottle.numberOfColorsInBottle != 0)
            {
                MoveBottle(
                    FirstBottle,
                    bottleDown
                );
            }
        }

        FirstBottle = null;
        SecondBottle = null;
    }

    private void MoveBottle(
        BottleController bottle,
        float verticalAmount
    )
    {
        if (bottle == null)
            return;

        Vector3 bottlePosition =
            bottle.transform.position;

        bottlePosition.y += verticalAmount;

        bottle.transform.position =
            bottlePosition;
    }

    private void CheckLevelCompletion()
    {
        if (levelFailed)
            return;

        if (allFull)
            return;

        if (bottles == null ||
            bottles.Length == 0)
            return;

        bool levelFinished = bottles.All(
            bottle =>
                bottle != null &&
                (
                    bottle.numberOfColorsInBottle == 0 ||
                    bottle.numberOfTopColorLayer == 4
                )
        );

        if (!levelFinished)
            return;

        allFull = true;

        // Level complete ho gaya to timer stop
        LevelTimer timer =
            GetComponent<LevelTimer>();

        if (timer != null)
        {
            timer.enabled = false;
        }

        StartCoroutine(
            CompleteLevelAfterDelay()
        );
    }

    private IEnumerator CompleteLevelAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        // Agar kisi wajah se fail ho gaya
        // to win panel show nahi hoga
        if (!levelFailed)
        {
            Win();
        }
    }

    private void Win()
    {
        if (!allFull || levelFailed)
            return;

        numberOfUnlockedLevel =
            PlayerPrefs.GetInt(
                "LevelIsUnlocked",
                0
            );

        if (numberOfUnlockedLevel <=
            levelToUnlock)
        {
            PlayerPrefs.SetInt(
                "LevelIsUnlocked",
                numberOfUnlockedLevel + 1
            );

            PlayerPrefs.Save();
        }

        if (LevelCompleted != null)
        {
            LevelCompleted.SetActive(true);
        }
        else
        {
            Debug.LogError(
                "LevelCompleted object is not assigned in GameController."
            );
        }
    }
}