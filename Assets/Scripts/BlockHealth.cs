using UnityEngine;

public class BlockHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 25f;

    [Header("Score")]
    [SerializeField] private int scoreOnDestroy = 100;

    public float CurrentHealth { get; private set; }

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (CurrentHealth <= 0f)
            return;

        CurrentHealth -= damage;

        Debug.Log(gameObject.name +
                  " took " + damage +
                  " damage. Health: " + CurrentHealth);

        if (CurrentHealth <= 0f)
        {
            CurrentHealth = 0f;
            DestroyBlock();
        }
    }

    private void DestroyBlock()
    {
        // Give score
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreOnDestroy);
        }

        Destroy(gameObject);
    }
}