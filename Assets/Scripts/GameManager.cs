using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ============================================================
    // SINGLETON
    // ============================================================

    public static GameManager Instance { get; private set; }


    // ============================================================
    // LEVEL SETTINGS
    // ============================================================

    [System.Serializable]
    public class LevelSettings
    {
        [Header("Level")]
        public GameObject levelObject;


        [Header("Jelly Limit")]
        public bool infiniteJellies = false;
        public int maxJellies = 5;


        [Header("Star Requirements")]
        public int oneStarScore = 5000;
        public int twoStarScore = 10000;
        public int threeStarScore = 20000;


        [Header("Unused Jelly Bonus")]
        public int unusedJellyBonus = 1000;
    }


    // ============================================================
    // LEVELS
    // ============================================================

    [Header("Levels")]
    public LevelSettings[] levels;


    // ============================================================
    // CURRENT LEVEL
    // ============================================================

    private LevelSettings currentLevel;
    private int currentLevelIndex = -1;


    // ============================================================
    // SCORE
    // ============================================================

    [Header("Current Score")]
    [SerializeField]
    private int score = 0;


    // ============================================================
    // JELLIES
    // ============================================================

    [Header("Jellies")]
    [SerializeField]
    private int jelliesUsed = 0;

    [SerializeField]
    private int jelliesRemaining = 0;


    // ============================================================
    // BLOCKS
    // ============================================================

    [Header("Blocks")]
    [SerializeField]
    private int blocksDestroyed = 0;


    // ============================================================
    // COMBO
    // ============================================================

    [Header("Combo")]
    [SerializeField]
    private int currentCombo = 0;

    [SerializeField]
    private int highestCombo = 0;


    // ============================================================
    // COMBO SETTINGS
    // ============================================================

    [Header("Combo Settings")]

    [Tooltip("How long another block has to be destroyed to continue the combo.")]
    public float comboTime = 1.5f;

    [Tooltip("Extra score awarded for every combo step.")]
    public float comboBonusPerStep = 0.5f;

    private float comboTimer = 0f;


    // ============================================================
    // GAME STATE
    // ============================================================

    private bool levelWon = false;


    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    // ============================================================
    // START
    // ============================================================

    private void Start()
    {
        FindActiveLevel();
    }


    // ============================================================
    // UPDATE
    // ============================================================

    private void Update()
    {
        UpdateComboTimer();

        if (currentLevel == null)
        {
            FindActiveLevel();
            return;
        }

        if (levelWon)
            return;

        CheckLevelComplete();
    }


    // ============================================================
    // FIND ACTIVE LEVEL
    // ============================================================

    void FindActiveLevel()
    {
        if (levels == null || levels.Length == 0)
        {
            Debug.LogWarning(
                "GameManager has no levels assigned!"
            );

            return;
        }


        int activeLevelCount = 0;
        int foundIndex = -1;


        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i] == null ||
                levels[i].levelObject == null)
                continue;


            if (levels[i].levelObject.activeSelf)
            {
                activeLevelCount++;
                foundIndex = i;
            }
        }


        if (activeLevelCount == 0)
        {
            Debug.LogWarning(
                "No active level found!"
            );

            return;
        }


        if (activeLevelCount > 1)
        {
            Debug.LogWarning(
                "More than one level is active!"
            );
        }


        if (currentLevelIndex != foundIndex)
        {
            SetupLevel(foundIndex);
        }
    }


    // ============================================================
    // SETUP LEVEL
    // ============================================================

    void SetupLevel(int index)
    {
        if (index < 0 ||
            index >= levels.Length)
            return;


        currentLevel =
            levels[index];

        currentLevelIndex =
            index;


        score = 0;

        jelliesUsed = 0;

        blocksDestroyed = 0;

        currentCombo = 0;

        highestCombo = 0;

        comboTimer = 0f;

        levelWon = false;


        if (currentLevel.infiniteJellies)
        {
            jelliesRemaining = -1;
        }
        else
        {
            jelliesRemaining =
                currentLevel.maxJellies;
        }


        Debug.Log(
            "===================================="
        );

        Debug.Log(
            "LEVEL " +
            (currentLevelIndex + 1) +
            " STARTED"
        );


        if (currentLevel.infiniteJellies)
        {
            Debug.Log(
                "Jellies: INFINITE"
            );
        }
        else
        {
            Debug.Log(
                "Jellies: " +
                jelliesRemaining
            );
        }

        Debug.Log(
            "===================================="
        );
    }


    // ============================================================
    // USE JELLY
    // ============================================================

    public bool UseJelly()
    {
        if (currentLevel != null &&
            currentLevel.infiniteJellies)
        {
            jelliesUsed++;

            Debug.Log(
                "Jelly used: " +
                jelliesUsed
            );

            return true;
        }


        if (jelliesRemaining <= 0)
        {
            Debug.Log(
                "NO JELLIES REMAINING!"
            );

            return false;
        }


        jelliesUsed++;

        jelliesRemaining--;


        Debug.Log(
            "Jelly used! Remaining: " +
            jelliesRemaining
        );


        return true;
    }


    // ============================================================
    // BLOCK DESTROYED
    // ============================================================

    public void BlockDestroyed(int baseScore)
    {
        if (levelWon)
            return;


        blocksDestroyed++;


        // ========================================================
        // START / CONTINUE COMBO
        // ========================================================

        if (comboTimer > 0f)
        {
            currentCombo++;
        }
        else
        {
            currentCombo = 1;
        }


        comboTimer =
            comboTime;


        if (currentCombo > highestCombo)
        {
            highestCombo =
                currentCombo;
        }


        // ========================================================
        // BASE SCORE
        // ========================================================

        int points =
            baseScore;


        // ========================================================
        // COMBO BONUS
        // ========================================================

        int comboBonus = 0;


        if (currentCombo > 1)
        {
            comboBonus =
                Mathf.RoundToInt(
                    baseScore *
                    comboBonusPerStep *
                    (currentCombo - 1)
                );
        }


        // ========================================================
        // FINAL POINTS
        // ========================================================

        int totalPoints =
            points +
            comboBonus;


        score +=
            totalPoints;


        // ========================================================
        // DEBUG
        // ========================================================

        if (currentCombo > 1)
        {
            Debug.Log(
                "🔥 COMBO x" +
                currentCombo +
                " | Base: +" +
                points +
                " | Combo Bonus: +" +
                comboBonus +
                " | Total: +" +
                totalPoints +
                " | Score: " +
                score
            );
        }
        else
        {
            Debug.Log(
                "BLOCK DESTROYED! +" +
                totalPoints +
                " | Score: " +
                score
            );
        }
    }


    // ============================================================
    // COMBO TIMER
    // ============================================================

    void UpdateComboTimer()
    {
        if (comboTimer <= 0f)
            return;


        comboTimer -=
            Time.deltaTime;


        if (comboTimer <= 0f)
        {
            comboTimer = 0f;

            if (currentCombo > 1)
            {
                Debug.Log(
                    "Combo ended at x" +
                    currentCombo
                );
            }

            currentCombo = 0;
        }
    }


    // ============================================================
    // CHECK LEVEL COMPLETE
    // ============================================================

    void CheckLevelComplete()
    {
        if (currentLevel == null ||
            currentLevel.levelObject == null)
            return;


        BlockHealth[] obstacles =
            currentLevel.levelObject
                .GetComponentsInChildren<BlockHealth>(true);


        if (obstacles.Length == 0)
        {
            WinLevel();
            return;
        }


        int remaining = 0;


        foreach (BlockHealth obstacle in obstacles)
        {
            if (obstacle != null)
            {
                remaining++;
            }
        }


        if (remaining == 0)
        {
            WinLevel();
        }
    }


    // ============================================================
    // WIN LEVEL
    // ============================================================

    void WinLevel()
    {
        if (levelWon)
            return;


        levelWon = true;


        int jellyBonus = 0;


        if (!currentLevel.infiniteJellies)
        {
            jellyBonus =
                jelliesRemaining *
                currentLevel.unusedJellyBonus;


            score +=
                jellyBonus;
        }


        int stars =
            CalculateStars();


        Debug.Log(
            "===================================="
        );

        Debug.Log(
            "             GAME WON!"
        );

        Debug.Log(
            "===================================="
        );

        Debug.Log(
            "Level: " +
            (currentLevelIndex + 1)
        );

        Debug.Log(
            "Final Score: " +
            score
        );

        Debug.Log(
            "Blocks Destroyed: " +
            blocksDestroyed
        );

        Debug.Log(
            "Jellies Used: " +
            jelliesUsed
        );


        if (currentLevel.infiniteJellies)
        {
            Debug.Log(
                "Jellies Remaining: INFINITE"
            );
        }
        else
        {
            Debug.Log(
                "Jellies Remaining: " +
                jelliesRemaining
            );

            Debug.Log(
                "Unused Jelly Bonus: +" +
                jellyBonus
            );
        }


        Debug.Log(
            "Highest Combo: x" +
            highestCombo
        );

        Debug.Log(
            "Stars: " +
            stars
        );

        Debug.Log(
            "===================================="
        );
    }


    // ============================================================
    // STARS
    // ============================================================

    int CalculateStars()
    {
        if (currentLevel == null)
            return 0;


        if (score >= currentLevel.threeStarScore)
            return 3;


        if (score >= currentLevel.twoStarScore)
            return 2;


        if (score >= currentLevel.oneStarScore)
            return 1;


        return 0;
    }


    // ============================================================
    // GETTERS
    // ============================================================

    public int GetStars()
    {
        return CalculateStars();
    }


    public int GetScore()
    {
        return score;
    }


    public int GetJelliesUsed()
    {
        return jelliesUsed;
    }


    public int GetJelliesRemaining()
    {
        return jelliesRemaining;
    }


    public int GetCurrentCombo()
    {
        return currentCombo;
    }


    public int GetHighestCombo()
    {
        return highestCombo;
    }


    public int GetCurrentLevel()
    {
        return currentLevelIndex;
    }


    // ============================================================
    // LOAD LEVEL
    // ============================================================

    public void LoadLevel(int levelIndex)
    {
        if (levels == null ||
            levels.Length == 0)
        {
            Debug.LogWarning(
                "No levels assigned!"
            );

            return;
        }


        if (levelIndex < 0 ||
            levelIndex >= levels.Length)
        {
            Debug.LogWarning(
                "Invalid level index!"
            );

            return;
        }


        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i] != null &&
                levels[i].levelObject != null)
            {
                levels[i].levelObject.SetActive(false);
            }
        }


        levels[levelIndex]
            .levelObject
            .SetActive(true);


        SetupLevel(levelIndex);
    }


    // ============================================================
    // NEXT LEVEL
    // ============================================================

    public void NextLevel()
    {
        int nextLevel =
            currentLevelIndex + 1;


        if (nextLevel >= levels.Length)
        {
            Debug.Log(
                "NO MORE LEVELS!"
            );

            return;
        }


        LoadLevel(nextLevel);
    }


    // ============================================================
    // PREVIOUS LEVEL
    // ============================================================

    public void PreviousLevel()
    {
        int previousLevel =
            currentLevelIndex - 1;


        if (previousLevel < 0)
        {
            Debug.Log(
                "Already on first level."
            );

            return;
        }


        LoadLevel(previousLevel);
    }


    // ============================================================
    // RESTART
    // ============================================================

    public void RestartLevel()
    {
        if (currentLevelIndex < 0)
            return;


        LoadLevel(currentLevelIndex);
    }
}