using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelCompleteUI : MonoBehaviour
{
    // ============================================================
    // POPUP ROOT
    // ============================================================

    [Header("Popup Root")]
    [SerializeField]
    private GameObject panel;


    // ============================================================
    // TEXT
    // ============================================================

    [Header("Text")]

    [SerializeField]
    private TMP_Text titleText;

    [SerializeField]
    private TMP_Text starsText;

    [SerializeField]
    private TMP_Text scoreText;

    [SerializeField]
    private TMP_Text blocksText;

    [SerializeField]
    private TMP_Text jelliesText;

    [SerializeField]
    private TMP_Text jellyBonusText;

    [SerializeField]
    private TMP_Text comboText;


    // ============================================================
    // BUTTONS
    // ============================================================

    [Header("Buttons")]

    [SerializeField]
    private Button nextLevelButton;

    [SerializeField]
    private Button restartButton;

    [SerializeField]
    private Button levelSelectButton;


    // ============================================================
    // SCENE SETTINGS
    // ============================================================

    [Header("Scene Settings")]

    [SerializeField]
    private string levelSelectSceneName = "LevelSelect";


    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        // IMPORTANT:
        // Do NOT hide the panel here.
        //
        // This script may be on an object that is disabled
        // at the beginning of the scene.
        //
        // The GameManager will call ShowResults() when needed.


        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.RemoveListener(
                OnNextLevelPressed
            );

            nextLevelButton.onClick.AddListener(
                OnNextLevelPressed
            );
        }


        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(
                OnRestartPressed
            );

            restartButton.onClick.AddListener(
                OnRestartPressed
            );
        }


        if (levelSelectButton != null)
        {
            levelSelectButton.onClick.RemoveListener(
                OnLevelSelectPressed
            );

            levelSelectButton.onClick.AddListener(
                OnLevelSelectPressed
            );
        }
    }


    // ============================================================
    // SHOW RESULTS
    // ============================================================

    public void ShowResults(
        int score,
        int stars,
        int blocksDestroyed,
        int jelliesUsed,
        int jelliesRemaining,
        int jellyBonus,
        int highestCombo,
        bool infiniteJellies)
    {
        Debug.Log(
            "===================================="
        );

        Debug.Log(
            "SHOWING LEVEL COMPLETE UI"
        );

        Debug.Log(
            "Score: " +
            score
        );

        Debug.Log(
            "Stars: " +
            stars
        );

        Debug.Log(
            "===================================="
        );


        // ========================================================
        // TITLE
        // ========================================================

        if (titleText != null)
        {
            titleText.text =
                "LEVEL COMPLETE!";
        }


        // ========================================================
        // STARS
        // ========================================================

        if (starsText != null)
        {
            switch (stars)
            {
                case 3:

                    starsText.text =
                        "★★★";

                    break;


                case 2:

                    starsText.text =
                        "★★☆";

                    break;


                case 1:

                    starsText.text =
                        "★☆☆";

                    break;


                default:

                    starsText.text =
                        "☆☆☆";

                    break;
            }
        }


        // ========================================================
        // SCORE
        // ========================================================

        if (scoreText != null)
        {
            scoreText.text =
                "Score: " +
                score.ToString("N0");
        }


        // ========================================================
        // BLOCKS
        // ========================================================

        if (blocksText != null)
        {
            blocksText.text =
                "Blocks Destroyed: " +
                blocksDestroyed;
        }


        // ========================================================
        // JELLIES
        // ========================================================

        if (jelliesText != null)
        {
            if (infiniteJellies)
            {
                jelliesText.text =
                    "Jellies Used: " +
                    jelliesUsed;
            }
            else
            {
                int totalJellies =
                    jelliesUsed +
                    jelliesRemaining;


                jelliesText.text =
                    "Jellies Used: " +
                    jelliesUsed +
                    " / " +
                    totalJellies;
            }
        }


        // ========================================================
        // JELLY BONUS
        // ========================================================

        if (jellyBonusText != null)
        {
            if (infiniteJellies)
            {
                jellyBonusText.text =
                    "Unused Jelly Bonus: N/A";
            }
            else
            {
                jellyBonusText.text =
                    "Unused Jelly Bonus: +" +
                    jellyBonus.ToString("N0");
            }
        }


        // ========================================================
        // HIGHEST COMBO
        // ========================================================

        if (comboText != null)
        {
            comboText.text =
                "Highest Combo: x" +
                highestCombo;
        }


        // ========================================================
        // NEXT LEVEL BUTTON
        // ========================================================

        if (nextLevelButton != null)
        {
            if (GameManager.Instance != null)
            {
                int currentLevel =
                    GameManager.Instance.GetCurrentLevel();

                int levelCount =
                    GameManager.Instance.GetLevelCount();


                if (currentLevel >= levelCount - 1)
                {
                    nextLevelButton.gameObject
                        .SetActive(false);
                }
                else
                {
                    nextLevelButton.gameObject
                        .SetActive(true);
                }
            }
        }


        // ========================================================
        // SHOW POPUP
        // ========================================================

        if (panel != null)
        {
            panel.SetActive(true);
        }
        else
        {
            Debug.LogError(
                "LevelCompleteUI: PANEL IS NOT ASSIGNED!"
            );
        }
    }


    // ============================================================
    // NEXT LEVEL
    // ============================================================

    private void OnNextLevelPressed()
    {
        Debug.Log(
            "NEXT LEVEL PRESSED"
        );


        if (GameManager.Instance == null)
        {
            Debug.LogError(
                "GameManager.Instance is null!"
            );

            return;
        }


        GameManager.Instance.NextLevel();


        if (panel != null)
        {
            panel.SetActive(false);
        }
    }


    // ============================================================
    // RESTART
    // ============================================================

    private void OnRestartPressed()
    {
        Debug.Log(
            "RESTART PRESSED"
        );


        if (GameManager.Instance == null)
        {
            Debug.LogError(
                "GameManager.Instance is null!"
            );

            return;
        }


        GameManager.Instance.RestartLevel();


        if (panel != null)
        {
            panel.SetActive(false);
        }
    }


    // ============================================================
    // LEVEL SELECT
    // ============================================================

    private void OnLevelSelectPressed()
    {
        Debug.Log(
            "RETURNING TO LEVEL SELECT"
        );


        SceneManager.LoadScene(
            levelSelectSceneName
        );
    }
}