using UnityEngine;

public class GameLevelManager : MonoBehaviour
{
    [Header("Levels")]
    [SerializeField] private GameObject level1;
    [SerializeField] private GameObject level2;
    [SerializeField] private GameObject level3;
    [SerializeField] private GameObject level4;

    private void Start()
    {
        ActivateSelectedLevel();
    }

    private void ActivateSelectedLevel()
    {
        // Turn ALL levels off first
        level1.SetActive(false);
        level2.SetActive(false);
        level3.SetActive(false);
        level4.SetActive(false);

        // Activate the selected level
        switch (LevelSelectManager.SelectedLevel)
        {
            case 1:
                level1.SetActive(true);
                break;

            case 2:
                level2.SetActive(true);
                break;

            case 3:
                level3.SetActive(true);
                break;

            case 4:
                level4.SetActive(true);
                break;

            default:
                Debug.LogWarning("No level was selected!");

                // Optional: default to Level 1
                level1.SetActive(true);
                break;
        }

        Debug.Log("Activated Level: " + LevelSelectManager.SelectedLevel);
    }
}