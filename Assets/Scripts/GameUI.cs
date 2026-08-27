using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("UI Text")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text comboText;
    [SerializeField] private TMP_Text jellyText;


    private void Update()
    {
        if (GameManager.Instance == null)
            return;


        UpdateScore();
        UpdateCombo();
        UpdateJellies();
    }


    // ============================================================
    // SCORE
    // ============================================================

    private void UpdateScore()
    {
        int score =
            GameManager.Instance.GetScore();


        if (scoreText != null)
        {
            scoreText.text =
                "Score: " +
                score.ToString("N0");
        }
    }


    // ============================================================
    // COMBO
    // ============================================================

    private void UpdateCombo()
    {
        int combo =
            GameManager.Instance.GetCurrentCombo();


        if (comboText == null)
            return;


        // No active combo
        if (combo <= 1)
        {
            comboText.text = "";
            return;
        }


        int comboBonus =
            GameManager.Instance.GetLastComboBonus();


        comboText.text =
            "COMBO x" +
            combo +
            "   |   BONUS +" +
            comboBonus.ToString("N0");
    }


    // ============================================================
    // JELLIES
    // ============================================================

    private void UpdateJellies()
    {
        int jellies =
            GameManager.Instance.GetJelliesRemaining();


        if (jellyText == null)
            return;


        // Infinite jellies
        if (jellies < 0)
        {
            jellyText.text =
                "Jellies: ∞";
        }
        else
        {
            jellyText.text =
                "Jellies: " +
                jellies;
        }
    }
}