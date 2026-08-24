using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int Score { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddScore(int amount)
    {
        Score += amount;

        Debug.Log("Score +" + amount + " | Total Score: " + Score);
    }

    public void ResetScore()
    {
        Score = 0;

        Debug.Log("Score reset");
    }
}