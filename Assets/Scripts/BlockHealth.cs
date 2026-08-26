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

    [Tooltip("How long the destroyed block remains as a soft body.")]
    public float destroyedSoftBodyTime = 1.5f;

    private JellyBlock jellyBlock;


    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        currentHealth =
            maxHealth;


        jellyBlock =
            GetComponent<JellyBlock>();
    }


    // ============================================================
    // DAMAGE
    // ============================================================

    public void TakeDamage(float damage)
    {
        if (destroyed)
            return;


        if (damage <= 0f)
            return;


        currentHealth -=
            damage;


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
        if (destroyed)
            return;


        destroyed = true;


        Debug.Log(
            gameObject.name +
            " destroyed!"
        );


        // ========================================================
        // SCORE
        // ========================================================

        if (GameManager.Instance != null)
        {
            GameManager.Instance.BlockDestroyed(
                scoreValue
            );
        }


        // ========================================================
        // ACTIVATE SOFT BODY
        // ========================================================

        if (jellyBlock != null)
        {
            jellyBlock.ActivateJelly(
                Vector2.zero
            );


            // Keep the soft body alive long enough
            // to hit other blocks.
            Destroy(
                gameObject,
                destroyedSoftBodyTime
            );
        }
        else
        {
            // If there is no soft body,
            // destroy normally.
            Destroy(
                gameObject,
                0.05f
            );
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