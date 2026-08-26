using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    public static int SelectedLevel { get; private set; }

    [Header("Game Scene")]
    [SerializeField] private string gameSceneName = "Game";

    public void SelectLevel1()
    {
        SelectLevel(1);
    }

    public void SelectLevel2()
    {
        SelectLevel(2);
    }

    public void SelectLevel3()
    {
        SelectLevel(3);
    }

    public void SelectLevel4()
    {
        SelectLevel(4);
    }

    private void SelectLevel(int level)
    {
        SelectedLevel = level;

        Debug.Log("Selected Level: " + level);

        SceneManager.LoadScene(gameSceneName);
    }
}