using UnityEngine;

public class BlockBone : MonoBehaviour
{
    // ============================================================
    // BLOCK
    // ============================================================

    private BlockHealth blockHealth;
    private JellyBlock jellyBlock;


    // ============================================================
    // IMPACT DAMAGE
    // ============================================================

    [Header("Impact Damage")]

    public float minimumImpactVelocity = 2f;

    public float maximumImpactVelocity = 10f;

    public float maximumImpactDamage = 40f;


    // ============================================================
    // DAMAGE COOLDOWN
    // ============================================================

    [Header("Damage Cooldown")]

    public float damageCooldown = 0.15f;

    private float lastDamageTime = -999f;


    // ============================================================
    // SOFT BODY
    // ============================================================

    [Header("Soft Body Activation")]

    public float softBodyImpactVelocity = 4f;


    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        blockHealth =
            GetComponentInParent<BlockHealth>();

        jellyBlock =
            GetComponentInParent<JellyBlock>();


        if (blockHealth == null)
        {
            Debug.LogWarning(
                "BlockBone could not find BlockHealth!"
            );
        }


        if (jellyBlock == null)
        {
            Debug.LogWarning(
                "BlockBone could not find JellyBlock!"
            );
        }
    }


    // ============================================================
    // COLLISION
    // ============================================================

    private void OnCollisionEnter2D(
        Collision2D collision
    )
    {
        if (blockHealth == null)
            return;


        if (blockHealth.IsDestroyed())
            return;


        float impactVelocity =
            collision.relativeVelocity.magnitude;


        // ========================================================
        // ACTIVATE THIS BLOCK'S SOFT BODY
        // ========================================================

        if (
            jellyBlock != null &&
            impactVelocity >= softBodyImpactVelocity
        )
        {
            jellyBlock.ActivateJelly(
                collision.relativeVelocity
            );
        }


        // ========================================================
        // JELLY
        // ========================================================

        JellyBone jellyBone =
            collision.collider.GetComponent<JellyBone>();


        if (jellyBone != null)
        {
            // JellyDamage handles this.
            return;
        }


        // ========================================================
        // FIND OTHER BLOCK
        // ========================================================

        BlockBone otherBlockBone =
            collision.collider.GetComponent<BlockBone>();


        if (otherBlockBone == null)
            return;


        BlockHealth otherBlock =
            otherBlockBone.GetBlockHealth();


        if (otherBlock == null)
            return;


        if (otherBlock == blockHealth)
            return;


        // ========================================================
        // DAMAGE THE BLOCK THAT WAS HIT
        // ========================================================

        if (
            impactVelocity <
            minimumImpactVelocity
        )
        {
            return;
        }


        // The collision happened on THIS block's bone.
        //
        // We want the OTHER block to take the damage.

        otherBlockBone.ApplyImpactDamage(
            impactVelocity
        );
    }


    // ============================================================
    // APPLY IMPACT DAMAGE
    // ============================================================

    public void ApplyImpactDamage(
        float impactVelocity
    )
    {
        if (blockHealth == null)
            return;


        if (blockHealth.IsDestroyed())
            return;


        // ========================================================
        // COOLDOWN
        // ========================================================

        if (
            Time.time -
            lastDamageTime <
            damageCooldown
        )
        {
            return;
        }


        lastDamageTime =
            Time.time;


        // ========================================================
        // DAMAGE CALCULATION
        // ========================================================

        float velocityPercent =
            Mathf.InverseLerp(
                minimumImpactVelocity,
                maximumImpactVelocity,
                impactVelocity
            );


        float damage =
            Mathf.Lerp(
                0f,
                maximumImpactDamage,
                velocityPercent
            );


        if (damage <= 0f)
            return;


        // ========================================================
        // DAMAGE
        // ========================================================

        blockHealth.TakeDamage(
            damage
        );


        Debug.Log(
            gameObject.name +
            " received " +
            damage +
            " impact damage."
        );
    }


    // ============================================================
    // GET BLOCK HEALTH
    // ============================================================

    public BlockHealth GetBlockHealth()
    {
        return blockHealth;
    }
}