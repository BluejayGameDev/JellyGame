using UnityEngine;

public class BlockHealth : MonoBehaviour
{
    // ============================================================
    // HEALTH
    // ============================================================

    [Header("Health")]

    public float maxHealth = 100f;

    private float currentHealth;


    // ============================================================
    // SCORE
    // ============================================================

    [Header("Score")]

    public int scoreValue = 100;


    // ============================================================
    // DESTROYED STATE
    // ============================================================

    private bool destroyed = false;


    // ============================================================
    // SOFT BODY
    // ============================================================

    [Header("Destroyed Soft Body")]
    public float destroyedSoftBodyTime = 1.5f;

    private JellyBlock jellyBlock;

    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        currentHealth = maxHealth;

        jellyBlock = GetComponent<JellyBlock>();
    }


    // ============================================================
    // DAMAGE
    // ============================================================

    public void TakeDamage(float damage)
    {
        if (destroyed) return;

        if (damage <= 0f) return;

        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }


    // ============================================================
    // DIE
    // ============================================================

    private void Die()
    {
        if (destroyed) return;

        destroyed = true;

        // ========================================================
        // SCORE
        // ========================================================

        if (GameManager.Instance != null)
        {
            GameManager.Instance.BlockDestroyed(scoreValue);
        }


        // ========================================================
        // ACTIVATE SOFT BODY
        // ========================================================

        if (jellyBlock != null)
        {
            jellyBlock.ActivateJelly(Vector2.zero);


            Destroy(gameObject, destroyedSoftBodyTime);
        }
        else
        {
            Destroy(gameObject, 0.05f);
        }
    }


    // ============================================================
    // HEALTH GETTERS
    // ============================================================

    public float GetHealth()
    {
        return currentHealth;
    }


    public float GetMaxHealth()
    {
        return maxHealth;
    }


    public bool IsDestroyed()
    {
        return destroyed;
    }
}